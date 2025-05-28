using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;
using static RenderMentor.Models.Social;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class SubscriptionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(TrialOnlineVM trialOnlineVM)
        {
            trialOnlineVM.TrialOnline = _unitOfWork.HomeContent.GetFirstOrDefault().TrialOnline;
            var subscriptions = _unitOfWork.Subscription.GetAll();
            trialOnlineVM.Subscriptions = subscriptions;
            return View(trialOnlineVM);
        }        

        public IActionResult Details(int? id)
        {
            Subscription subscription;
            if (id == null || id == 0)
            {
                subscription = new Subscription();
            }
            else
            {
                subscription = _unitOfWork.Subscription.Get(id.Value);
            }
            return View(subscription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                if (subscription.Id == 0)
                {
                    _unitOfWork.Subscription.Add(subscription);
                }
                else
                {
                    _unitOfWork.Subscription.Update(subscription);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(subscription);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Subscription.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işlemi sırasında hata oluştu." });
            }
            _unitOfWork.Subscription.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Üyelik paketi başarıyla silindi." });
        }

        public IActionResult List()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Student.GetAll(s => s.SubscribeActive == true);
            var packageName = _unitOfWork.Subscription.GetFirstOrDefault().Name;
            var activePackages = new List<ActivePackageVM>();
            foreach (var item in allObj)
            {
                var package = new ActivePackageVM()
                {
                    StartDate = item.SubscribeStartDate,
                    EndDate = item.SubscribeExpireDate,
                    PackageName = packageName,
                    DaysToExpire = Math.Round((item.SubscribeExpireDate - DateTime.Now).TotalDays),
                    Student = _unitOfWork.Student.GetFirstOrDefault(i => i.Id == item.Id, includeProperties: "ApplicationUser")
                };
                package.StudentName = package.Student.ApplicationUser.Name;
                package.StudentEmail = package.Student.ApplicationUser.Email;
                package.UserId = package.Student.ApplicationUser.Id;
                activePackages.Add(package);
            }
            return Json(new { data = activePackages });
        }

        [HttpPost]
        public IActionResult TrialOnline()
        {
            var objFromDb = _unitOfWork.HomeContent.GetFirstOrDefault();
            if (objFromDb.TrialOnline)
            {
                // user is currently locked, we will unlock them
                objFromDb.TrialOnline = false;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Deneme üyeliği yayından kaldırıldı." });
            }
            else
            {
                objFromDb.TrialOnline = true;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Deneme üyeliği yayına alındı." });
            }
        }

        #endregion
    }
}