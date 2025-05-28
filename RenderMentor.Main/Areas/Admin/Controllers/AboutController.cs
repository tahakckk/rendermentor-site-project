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
    public class AboutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AboutController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            About about = _unitOfWork.About.GetFirstOrDefault();
            return View(about);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(About about, IFormFile PageImage, IFormFile PageBg)
        {
            var objFromDb = _unitOfWork.About.GetFirstOrDefault(tracking: false);

            if (ModelState.IsValid)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "content";
                if (PageImage != null)
                {
                    string fileName = "render-mentor-about";
                    ImageUploader.UploadImage(webRoothPath, folderName, PageImage, objFromDb.PageImage, fileName);
                    var extension = Path.GetExtension(PageImage.FileName);
                    about.PageImage = @"\images\" + folderName + @"\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    about.PageImage = objFromDb.PageImage;
                }
                if (PageBg != null)
                {
                    string fileName = "render-mentor-about-bg";
                    ImageUploader.UploadImage(webRoothPath, folderName, PageBg, objFromDb.PageBg, fileName);
                    var extension = Path.GetExtension(PageBg.FileName);
                    about.PageBg = @"\images\" + folderName + @"\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    about.PageBg = objFromDb.PageBg;
                }

                _unitOfWork.About.Update(about);
                _unitOfWork.Save();

                return View(about);
            }

            return View(about);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMember(IFormFile AvatarImageFile, string AvatarImage, string memberName, string memberTitle, string memberDesc)
        {
            var allObj = _unitOfWork.Team.GetAll();
            var countObj = allObj.Count();
            var member = new Team()
            {
                Name = memberName,
                Title = memberTitle,
                Desc = memberDesc,
                AvatarImage = AvatarImage,
                ListOrder = countObj + 1
            };
            if (AvatarImageFile != null)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "team";
                string fileName = LinkConverter.CreateUrl(memberName);
                ImageUploader.UploadImage(webRoothPath, folderName, AvatarImageFile, member.AvatarImage, fileName);
                var extension = Path.GetExtension(AvatarImageFile.FileName);
                member.AvatarImage = @"\images\" + folderName + @"\" + fileName + extension;
            }
            _unitOfWork.Team.Add(member);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Üye eklendi." });
        }

        [HttpGet]
        [Route("findMember/{id}")]
        public IActionResult FindMember(int id)
        {
            var member = _unitOfWork.Team.Get(id);
            return new JsonResult(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMember(int memberId, IFormFile AvatarImageFile, string AvatarImage, string memberName, string memberTitle, string memberDesc)
        {
            var member = _unitOfWork.Team.Get(memberId);
            member.Name = memberName;
            member.Title = memberTitle;
            member.Desc = memberDesc;
            member.AvatarImage = AvatarImage;
            string webRoothPath = _hostEnvironment.WebRootPath;
            string folderName = "team";
            if (AvatarImageFile != null)
            {
                string fileName = LinkConverter.CreateUrl(memberName);
                ImageUploader.UploadImage(webRoothPath, folderName, AvatarImageFile, member.AvatarImage, fileName);
                var extension = Path.GetExtension(AvatarImageFile.FileName);

                // When we changed the avatar image, browsers still showing the old image because if file name never changes browser uses the image from cache. Filenames do stay unchanged in this method when member names stay the same. In order to show immediate change in user interface we need to prevent caching. We achieve this by creating a random number in 3 digits which appends the file name. Final output will be like; "images/team/member-name.jpg?v=123".
                string noCache = "?v=" + RandomGenerator.GenerateRandomNo(100, 999);

                member.AvatarImage = @"\images\" + folderName + @"\" + fileName + extension + noCache;
            }
            _unitOfWork.Team.Update(member);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Üye güncellendi." });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetTeam()
        {
            var allObj = _unitOfWork.Team.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult DeleteMember(int id)
        {
            var allObj = _unitOfWork.Team.GetAll();
            var objFromDb = _unitOfWork.Team.Get(id);
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
            _unitOfWork.Team.Remove(objFromDb);
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

        public void TeamListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.Team.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        #endregion
    }
}