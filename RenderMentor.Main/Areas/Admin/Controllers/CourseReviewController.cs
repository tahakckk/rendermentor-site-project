using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CourseReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }       

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.CourseReview.GetAll(includeProperties: "ApplicationUser,Course");
            return Json(new { data = allObj });
        }        

        [HttpPost]
        public IActionResult PublishToggle([FromBody] int id)
        {
            var objFromDb = _unitOfWork.CourseReview.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Yayınlama durumunda hata oluştu." });
            }
            if (objFromDb.Published == true)
            {
                // user is currently locked, we will unlock them
                objFromDb.Published = false;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Yorum yayından kaldırıldı." });
            }
            else
            {
                objFromDb.Published = true;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Yorum yayına alındı." });
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.CourseReview.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }

            _unitOfWork.CourseReview.Remove(id);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        [HttpGet]
        [Route("findCourseReview/{id}")]
        public IActionResult findCourseReview(int id)
        {
            var courseReview = _unitOfWork.CourseReview.GetFirstOrDefault(i => i.Id == id, includeProperties: "ApplicationUser,Course");
            return new JsonResult(courseReview);
        }

        #endregion
    }
}