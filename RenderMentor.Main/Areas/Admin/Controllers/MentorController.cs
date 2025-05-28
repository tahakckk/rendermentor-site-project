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
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class MentorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MentorController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            MentorPage mentorPage = _unitOfWork.MentorPage.GetFirstOrDefault();
            if (mentorPage == null)
            {
                mentorPage = new MentorPage();
            }
            return View(mentorPage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(MentorPage mentorPage, IFormFile PageBg)
        {
            var objFromDb = _unitOfWork.MentorPage.GetFirstOrDefault(tracking: false);

            if (ModelState.IsValid)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "mentors";                
                if (PageBg != null)
                {
                    string fileName = "render-mentor-mentors-bg";
                    ImageUploader.UploadImage(webRoothPath, folderName, PageBg, objFromDb.PageBg, fileName);
                    var extension = Path.GetExtension(PageBg.FileName);
                    mentorPage.PageBg = @"\images\" + folderName + @"\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    mentorPage.PageBg = objFromDb.PageBg;
                }

                _unitOfWork.MentorPage.Update(mentorPage);
                _unitOfWork.Save();

                return View(mentorPage);
            }

            return View(mentorPage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMentor(IFormFile AvatarImageFile, string AvatarImage, string mentorName, string mentorTitle, string mentorDesc)
        {
            var allObj = _unitOfWork.Mentor.GetAll();
            var countObj = allObj.Count();
            var mentor = new Mentor()
            {
                Name = mentorName,
                Title = mentorTitle,
                Desc = mentorDesc,
                AvatarImage = AvatarImage,
                ListOrder = countObj + 1
            };
            if (AvatarImageFile != null)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "mentors";
                string fileName = LinkConverter.CreateUrl(mentorName);
                ImageUploader.UploadImage(webRoothPath, folderName, AvatarImageFile, mentor.AvatarImage, fileName);
                var extension = Path.GetExtension(AvatarImageFile.FileName);
                mentor.AvatarImage = @"\images\" + folderName + @"\" + fileName + extension;
            }
            _unitOfWork.Mentor.Add(mentor);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Mentor eklendi." });
        }

        [HttpGet]
        [Route("findMentor/{id}")]
        public IActionResult FindMentor(int id)
        {
            var mentor = _unitOfWork.Mentor.Get(id);
            return new JsonResult(mentor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMentor(int mentorId, IFormFile AvatarImageFile, string AvatarImage, string mentorName, string mentorTitle, string mentorDesc)
        {
            var mentor = _unitOfWork.Mentor.Get(mentorId);
            var allObj = _unitOfWork.Mentor.GetAll();
            mentor.Name = mentorName;
            mentor.Title = mentorTitle;
            mentor.Desc = mentorDesc;
            mentor.AvatarImage = AvatarImage;
            string webRoothPath = _hostEnvironment.WebRootPath;
            string folderName = "mentors";
            if (AvatarImageFile != null)
            {
                string fileName = LinkConverter.CreateUrl(mentorName);
                ImageUploader.UploadImage(webRoothPath, folderName, AvatarImageFile, mentor.AvatarImage, fileName);
                var extension = Path.GetExtension(AvatarImageFile.FileName);

                // When we changed the avatar image, browsers still showing the old image because if file name never changes browser uses the image from cache. Filenames do stay unchanged in this method when mentor names stay the same. In order to show immediate change in user interface we need to prevent caching. We achieve this by creating a random number in 3 digits which appends the file name. Final output will be like; "images/mentors/mentor-name.jpg?v=123".
                string noCache = "?v=" + RandomGenerator.GenerateRandomNo(100, 999);

                mentor.AvatarImage = @"\images\" + folderName + @"\" + fileName + extension + noCache;
            }
            _unitOfWork.Mentor.Update(mentor);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Mentor güncellendi." });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetMentors()
        {
            var allObj = _unitOfWork.Mentor.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult DeleteMentor(int id)
        {
            var allObj = _unitOfWork.Mentor.GetAll();
            var objFromDb = _unitOfWork.Mentor.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            if (objFromDb.AvatarImage != null)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRoothPath, objFromDb.AvatarImage.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _unitOfWork.Mentor.Remove(objFromDb);
            foreach (var item in allObj)
            {
                if (item.ListOrder > objFromDb.ListOrder)
                {
                    item.ListOrder--;
                }
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        public void MentorListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.Mentor.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        #endregion
    }
}