using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            HomeContent homeContent = _unitOfWork.HomeContent.GetFirstOrDefault();            
            return View(homeContent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(HomeContent homeContent, IFormFile MentorImage)
        {
            var objFromDb = _unitOfWork.HomeContent.GetFirstOrDefault(tracking: false);

            if (ModelState.IsValid)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "instructors";
                if (MentorImage != null)
                {
                    string fileName = LinkConverter.CreateUrl(objFromDb.MentorName) + "-home";
                    ImageUploader.UploadImage(webRoothPath, folderName, MentorImage, objFromDb.MentorImage, fileName);
                    var extension = Path.GetExtension(MentorImage.FileName);
                    homeContent.MentorImage = @"\images\" + folderName + @"\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    homeContent.MentorImage = objFromDb.MentorImage;
                }

                _unitOfWork.HomeContent.Update(homeContent);
                _unitOfWork.Save();

                return View(homeContent);
            }

            return View(homeContent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSlide(IFormFile slideBgFile, string slideBg, string title, string desc, string buttonLink, string buttonText)
        {
            if (slideBgFile != null)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "slider";
                var allObj = _unitOfWork.HomeSlider.GetAll();
                var countObj = allObj.Count();
                var slide = new HomeSlider()
                {
                    Title = title,
                    Desc = desc,
                    SlideBg = slideBg,
                    ButtonLink = buttonLink,
                    ButtonText = buttonText,
                    ListOrder = countObj + 1
                };
                string newNumber = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToString();
                newNumber = newNumber.Replace("+", "7");
                string fileName = "rendermentor-slide-" + newNumber;
                ImageUploader.UploadImage(webRoothPath, folderName, slideBgFile, slide.SlideBg, fileName);
                var extension = Path.GetExtension(slideBgFile.FileName);
                slide.SlideBg = @"\images\" + folderName + @"\" + fileName + extension;

                _unitOfWork.HomeSlider.Add(slide);
                _unitOfWork.Save();
            }
            else
            {
                ModelState.AddModelError("", "Lütfen bir slayt görseli ekleyiniz.");
            }
            return RedirectToAction(nameof(Index));
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSlide(int slideId, IFormFile slideBgFile, string slideBg, string title, string desc, string buttonLink, string buttonText)
        {
            var slide = _unitOfWork.HomeSlider.Get(slideId);
            var allObj = _unitOfWork.HomeSlider.GetAll();
            var countObj = allObj.Count();
            slide.Title = title;
            slide.Desc = desc;
            slide.ButtonLink = buttonLink;
            slide.ButtonText = buttonText;
            slide.SlideBg = slideBg;
            string webRoothPath = _hostEnvironment.WebRootPath;
            string folderName = "slider";
            if (slideBgFile != null)
            {
                string newNumber = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToString();
                newNumber = newNumber.Replace("+", "7");
                string fileName = "rendermentor-slide-" + newNumber;
                ImageUploader.UploadImage(webRoothPath, folderName, slideBgFile, slide.SlideBg, fileName);
                var extension = Path.GetExtension(slideBgFile.FileName);
                slide.SlideBg = @"\images\" + folderName + @"\" + fileName + extension;
            }
            _unitOfWork.HomeSlider.Update(slide);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UploadReference(IFormFile ReferenceLogo, int id)
        {
            var allObj = _unitOfWork.Reference.GetAll();
            var countObj = allObj.Count();

            var reference = new Reference()
            {
                ListOrder = countObj + 1
            };
            if (ModelState.IsValid)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "references";
                if (ReferenceLogo != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ReferenceLogo.FileName);
                    ImageUploader.UploadImage(webRoothPath, folderName, ReferenceLogo, reference.Logo, fileName);
                    var extension = Path.GetExtension(ReferenceLogo.FileName);
                    reference.Logo = @"\images\" + folderName + @"\" + fileName + extension;
                }

                _unitOfWork.Reference.Add(reference);
                _unitOfWork.Save();
            }

            return Json(new { success = true, message = "Görsel galeriye eklendi." });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.HomeSlider.GetAll();
            //foreach (var slide in allObj)
            //{
            //    if (slide.Title == null)
            //    {
            //        slide.Title = "";
            //    }
            //}
            return Json(new { data = allObj });
        }

        [HttpGet]
        public IActionResult GetReferences(int id)
        {
            var allObj = _unitOfWork.Reference.GetAll();
            return Json(new { data = allObj });
        }

        [HttpGet]
        [Route("findSlide/{id}")]
        public IActionResult FindSlide(int id)
        {
            var slide = _unitOfWork.HomeSlider.Get(id);
            return new JsonResult(slide);
        }

        [HttpDelete]
        public IActionResult DeleteSlide(int id)
        {
            var allObj = _unitOfWork.HomeSlider.GetAll();
            var objFromDb = _unitOfWork.HomeSlider.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRoothPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRoothPath, objFromDb.SlideBg.TrimStart('\\'));            
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.HomeSlider.Remove(objFromDb);
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

        [HttpGet]
        public IActionResult FindPickedImages(int id)
        {
            var currentImages = _unitOfWork.SlideBg.GetAll().Where(i => i.HomeSliderId == id).ToList();
            return new JsonResult(currentImages);
        }

        [HttpPost("Admin/Home/PickImages/{pickedImages}/{id}")]
        public IActionResult PickImages(string pickedImages, int id)
        {
            string[] values = JsonConvert.DeserializeObject<string[]>(pickedImages);
            int[] idsArray = Array.ConvertAll(values, i => int.Parse(i));
            var currentImages = _unitOfWork.SlideBg.GetAll().Where(i => i.HomeSliderId == id).ToList();
            if (currentImages.Count() > 0)
            {
                var toDelete = currentImages.Where(i => !idsArray.Contains(i.Id)).ToList();
                foreach (var item in toDelete)
                {
                    item.HomeSliderId = null;
                }
            }

            var toPick = _unitOfWork.SlideBg.GetAll().Where(i => idsArray.Contains(i.Id)).ToList();
            foreach (var item in toPick)
            {
                item.HomeSliderId = id;
            }

            _unitOfWork.Save();
            return Json(new { success = true, message = "Toplu seçim işlemi başarılı." });
        }

        [HttpDelete]
        public IActionResult DeleteReference(int id)
        {
            var allObj = _unitOfWork.Reference.GetAll();
            var objFromDb = _unitOfWork.Reference.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRoothPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRoothPath, objFromDb.Logo.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Reference.Remove(objFromDb);
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

        public void SlideListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.HomeSlider.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        public void ReferenceListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.Reference.GetAll();
            Reference changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;
            _unitOfWork.Save();
        }

        #endregion
    }
}