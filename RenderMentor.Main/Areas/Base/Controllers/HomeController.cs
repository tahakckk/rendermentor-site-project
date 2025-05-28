using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Base.Controllers
{
    [Area("Base")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var homeContent = _unitOfWork.HomeContent.GetFirstOrDefault();
            if (homeContent == null)
            {
                homeContent = new HomeContent();
            }
            if (homeContent.MetaTitle != null)
            {
                ViewData["Title"] = homeContent.MetaTitle;
            }
            else
            {
                ViewData["Title"] = homeContent.SubTitle + homeContent.Title + "| RenderMentor";
            }
            ViewData["Description"] = homeContent.MetaDesc;
            return View(homeContent);
        }

        [Route("hakkimizda")]
        public IActionResult About()
        {
            var about = _unitOfWork.About.GetFirstOrDefault();
            if (about == null)
            {
                about = new About();
            }
            if (about.MetaTitle != null)
            {
                ViewData["Title"] = about.MetaTitle;
            }
            else
            {
                ViewData["Title"] = "Hakkımızda | RenderMentor";
            }
            ViewData["Description"] = about.MetaDesc;
            return View(about);
        }

        [Route("mentorlar")]
        public IActionResult Mentors()
        {
            var mentors = _unitOfWork.MentorPage.GetFirstOrDefault();
            if (mentors == null)
            {
                mentors = new MentorPage();
            }
            if (mentors.MetaTitle != null)
            {
                ViewData["Title"] = mentors.MetaTitle;
            }
            else
            {
                ViewData["Title"] = "Mentorlar | RenderMentor";
            }
            ViewData["Description"] = mentors.MetaDesc;
            return View(mentors);
        }

        [Route("uyelik-satin-al")]
        public IActionResult Subscription()
        {
            var subscriptions = _unitOfWork.Subscription.GetAll(s => s.IsActive == true);
            if (subscriptions == null || !subscriptions.Any())
            {
                subscriptions = new List<Subscription>();
            }
            
            var firstSubscription = subscriptions.FirstOrDefault();
            if (firstSubscription != null && firstSubscription.MetaTitle != null)
            {
                ViewData["Title"] = firstSubscription.MetaTitle;
            }
            else
            {
                ViewData["Title"] = "Üyelik Satın Al | RenderMentor";
            }
            ViewData["Description"] = firstSubscription?.MetaDesc;
            return View(subscriptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult AddToCart(int packageId)
        {
            var packageFromDb = _unitOfWork.Subscription.Get(packageId);
            if (packageFromDb == null)
            {
                return Json(new { success = false, message = "Paket bulunamadı." });
            }

            var course = _unitOfWork.Course.GetFirstOrDefault();
            if (course == null)
            {
                return Json(new { success = false, message = "Kurs bulunamadı." });
            }

            var cartobject = new ShoppingCart()
            {
                Id = 0,
                PackageId = packageId,
                CourseId = course.Id
            };

            if (packageFromDb.IsActive && ModelState.IsValid)
            {
                // then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bilgisi bulunamadı." });
                }

                var userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == claim.Value);
                if (userStudent == null)
                {
                    return Json(new { success = false, message = "Öğrenci bilgisi bulunamadı." });
                }

                cartobject.ApplicationUserId = claim.Value;

                if (userStudent.SubscribeActive)
                {
                    return Json(new { success = false, message = "Zaten bir üyelik pakedine sahipsiniz. Pakediniz sonlandıktan sonra yeni paket satın alabilirsiniz." });
                }

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartobject.ApplicationUserId && u.PackageId == cartobject.PackageId
                    , includeProperties: "Subscription"
                );
                var oldRecords = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartobject.ApplicationUserId).ToList();
                var cnt = oldRecords.Count();
                
                if (cartFromDb == null)
                {
                    foreach (var cart in oldRecords)
                    {
                        _unitOfWork.ShoppingCart.Remove(cart);
                        HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);
                    }
                    // no records exist in database for that product for that user
                    _unitOfWork.ShoppingCart.Add(cartobject);
                }
                _unitOfWork.Save();
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == cartobject.ApplicationUserId).ToList().Count();

                // HttpContext.Session.SetObject(SD.ssShoppingCart, cartobject);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                var redirectUrl = Url.Action("Index", "Cart");
                return Json(new { success = true, Url = redirectUrl });
            }
            else
            {
                return Json(new { success = false, message = "Sepete eklenemedi. Lütfen tekrar deneyin." });
            }
        }

        [Route("uyelikler-hakkinda")]
        public IActionResult Memberships()
        {
            var memberships = _unitOfWork.Memberships.GetFirstOrDefault();
            if (memberships.MetaTitle != null)
            {
                ViewData["Title"] = memberships.MetaTitle;
            }
            else
            {
                ViewData["Title"] = "Üyelikler Hakkında | RenderMentor";
            }
            ViewData["Description"] = memberships.MetaDesc;
            return View(memberships);
        }

        [Route("sikca-sorulan-sorular")]
        public IActionResult Faq()
        {
            var faq = _unitOfWork.Faq.GetAll().OrderBy(i => i.ListOrder);
            return View(faq);
        }

        [Route("iletisim")]
        public IActionResult Contact()
        {
            var contact = _unitOfWork.Contact.GetFirstOrDefault();
            if (contact.MetaTitle != null)
            {
                ViewData["Title"] = contact.MetaTitle;
            }
            else
            {
                ViewData["Title"] = "İletişim | RenderMentor";
            }
            ViewData["Description"] = contact.MetaDesc;
            return View(contact);
        }

        [Route("yasal-sozlesmeler")]
        public IActionResult ContractList()
        {
            var contractList = _unitOfWork.Contract.GetAll().OrderBy(i => i.ListOrder);
            return View(contractList);
        }

        [Route("yasal-sozlesmeler/{SEOUrl}")]
        public IActionResult ContractDetail(string SEOUrl)
        {
            var getContract = _unitOfWork.Contract.GetFirstOrDefault(i => i.SEOUrl == SEOUrl);
            if (getContract == null)
            {
                return NotFound();
            }
            return View(getContract);
        }

        [HttpPost]
        public async Task<IActionResult> DrawForFreeCourse(string email)
        {
            var allObj = _unitOfWork.DrawParticipant.GetAll();
            var objFromDb = _unitOfWork.DrawParticipant.GetFirstOrDefault(d => d.Email == email);
            if (objFromDb != null)
            {
                return Json(new { success = false, message = "Sayın " + email + ", e-posta adresiniz ile bu çekilişe katılım kaydınız mevcuttur." });
            }
            var drawParticipant = new DrawParticipant()
            {
                Email = email,
                SubmitDate = DateTime.Now
            };
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                drawParticipant.IsStudent = true;
                drawParticipant.StudentId = user.Id;
            }
            _unitOfWork.DrawParticipant.Add(drawParticipant);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Sayın " + email + ", çekilişe katıldınız. Kazanmanız halinde en kısa zamanda bilgilendirileceksiniz." });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
