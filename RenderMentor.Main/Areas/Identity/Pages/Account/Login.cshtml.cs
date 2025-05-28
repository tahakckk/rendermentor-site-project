using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RenderMentor.Models;
using RenderMentor.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public LoginModel(SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Lütfen emailinizi yazınız.")]
            [EmailAddress(ErrorMessage = "Lütfen geçerli bir mail adresi yazınız.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Lütfen parolanızı yazınız.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Beni hatırla?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Sistemde bu mail adresi ile kayıtlı kullanıcı bulunamadı.");
                    return Page();
                }

                var eMailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    int count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == user.Id).Count();
                    HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                    if (_userManager.IsInRoleAsync(user, SD.Role_Admin).Result)
                    {
                        int unanswered = _unitOfWork.LectureQuestion.GetAll(u => u.Answered == false).Count();
                        HttpContext.Session.SetInt32(SD.PendingQuestions, unanswered);
                    }

                    _logger.LogInformation("Kullanıcı giriş yaptı.");
                    if (returnUrl == "/")
                    {
                        if (_userManager.IsInRoleAsync(user, SD.Role_Admin).Result)
                        {
                            return LocalRedirect("/Admin/Dashboard");
                        }
                        return LocalRedirect("/Student/Student");
                    }
                    return LocalRedirect(returnUrl);
                }

                if (!eMailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email hesabınız doğrulanmamıştır. Lütfen postalarınızı kontrol ediniz.");
                    return Page();
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Kullanıcı hesabı dondurulmuştur.");
                    return RedirectToPage("./Lockout");
                }

                ModelState.AddModelError(string.Empty, "Parolanız uyuşmamaktadır. Lütfen tekrar kontrol ediniz.");
            }

            return Page();
        }
    }
}
