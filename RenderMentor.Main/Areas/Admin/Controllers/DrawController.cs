using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DrawController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DrawController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var isWinner = _unitOfWork.DrawParticipant.GetAll(d => d.IsWinner == true);
            var homeContent = _unitOfWork.HomeContent.GetFirstOrDefault();
            var drawVM = new DrawVM()
            {
                DrawOnline = homeContent.DrawOnline,
                WinnerEmails = new List<string>(),
                DrawTitle = homeContent.DrawTitle,
                DrawDesc = homeContent.DrawDesc,
                DrawSmallDesc = homeContent.DrawSmallDesc
            };
            if (isWinner.Count() > 0)
            {
                drawVM.HasWinner = true;
                foreach (var item in isWinner)
                {
                    drawVM.WinnerEmails.Add(item.Email);
                }
            }
            return View(drawVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DrawVM drawVM)
        {
            var homeContent = _unitOfWork.HomeContent.GetFirstOrDefault(tracking: false);

            if (ModelState.IsValid)
            {
                homeContent.DrawTitle = drawVM.DrawTitle;
                homeContent.DrawDesc = drawVM.DrawDesc;
                homeContent.DrawSmallDesc = drawVM.DrawSmallDesc;

                _unitOfWork.HomeContent.Update(homeContent);
                _unitOfWork.Save();

                return View(drawVM);
            }

            return View(drawVM);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.DrawParticipant.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.DrawParticipant.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            _unitOfWork.DrawParticipant.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        [HttpPost]
        public IActionResult StartDraw()
        {
            var count = _unitOfWork.DrawParticipant.GetAll().Count();
            if (count == 0)
            {
                return Json(new { success = false, message = "Çekiliş listesinde henüz katılımcı yok." });
            }
            var isWinner = _unitOfWork.DrawParticipant.GetAll(d => d.IsWinner == true);
            if (isWinner.Count() < 3)
            {
                var allObj = _unitOfWork.DrawParticipant.GetAll().Where(d => d.IsWinner == false);
                if (allObj.Count() == 0)
                {
                    return Json(new { success = false, message = "Çekiliş için yeterli kişi yok." });
                }
                var countObj = allObj.Count();
                Random rand = new Random();
                int offset = rand.Next(0, countObj);
                var winner = allObj.Skip(offset).FirstOrDefault();
                winner.IsWinner = true;
                _unitOfWork.Save();
                return Json(new { winner.Email, success = true, message = "Çekiliş kazananı belirlendi." });
            }
            else
            {
                return Json(new { success = false, message = "Bu çekilişte zaten tüm kazananları belirlediniz." });
            }
        }

        [HttpDelete]
        public IActionResult NewDraw()
        {
            var allObj = _unitOfWork.DrawParticipant.GetAll();
            _unitOfWork.DrawParticipant.RemoveRange(allObj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Tüm çekiliş listesi temizlendi." });
        }

        [HttpPost]
        public IActionResult DrawOnline()
        {
            var homeContent = _unitOfWork.HomeContent.GetFirstOrDefault();
            if (homeContent.DrawOnline)
            {
                // user is currently locked, we will unlock them
                homeContent.DrawOnline = false;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Çekiliş kampanyası yayından kaldırıldı." });
            }
            else
            {
                homeContent.DrawOnline = true;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Çekiliş kampanyası yayına alındı." });
            }
        }

        #endregion

    }
}