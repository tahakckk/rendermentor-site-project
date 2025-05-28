using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Utility;
using static RenderMentor.Models.ApplicationUser;

namespace RenderMentor.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email alanı gereklidir.")]
            [EmailAddress(ErrorMessage = "Lütfen geçerli bir mail adresi yazınız.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Parola alanı gereklidir.")]
            [StringLength(100, ErrorMessage = "{0} en az {2}, en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 8)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "Parolanız en az bir büyük ve bir küçük harf ile en az bir rakam içermelidir.")]
            [DataType(DataType.Password)]
            [Display(Name = "Parola")]
            public string Password { get; set; }

            [DataType(DataType.Password, ErrorMessage = "Parolanızın tekrarını yazınız.")]
            [Display(Name = "Parola tekrarı")]
            [Compare("Password", ErrorMessage = "Parolanız tekrarıyla uyuşmuyor.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "İsim alanı gereklidir.")]
            public string Name { get; set; }
            public Genders Gender { get; set; }

            [Phone(ErrorMessage = "Lütfen geçerli bir telefon numarası yazınız.")]
            public string PhoneNumber { get; set; }
            public int? CompanyId { get; set; }
            public string Role { get; set; }

            public IEnumerable<SelectListItem> CompanyList { get; set; }
            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            Input = new InputModel()
            {
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                RoleList = _roleManager.Roles.Where(u => u.Name != SD.Role_User_Individual).Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    Gender = Input.Gender,
                    PhoneNumber = Input.PhoneNumber,
                    CompanyId = Input.CompanyId,
                    Role = Input.Role,
                    CreateDate = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Instructor))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Instructor));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_User_Company))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Company));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_User_Individual))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Individual));
                    }

                    if (user.Role == null)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_User_Individual);
                        user.Role = SD.Role_User_Individual;
                    }
                    else
                    {
                        if (user.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_User_Company);
                        }
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }

                    var userStudent = new Models.Student()
                    {
                        UserId = user.Id
                    };
                    _unitOfWork.Student.Add(userStudent);
                    _unitOfWork.Save();

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Hesabınızı onaylayın.",
                        $"Sayın {Input.Name}, lütfen RenderMentor hesabınızı onaylamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>linke</a> tıklayınız.");

                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            // admin is registered a new user
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
