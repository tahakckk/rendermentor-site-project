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
    [Authorize(Roles = SD.Role_Admin)]
    [Area("Admin")]
    public class MediaGalleryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MediaGalleryController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SlideBg()
        {
            return View();
        }

        public IActionResult CourseCover()
        {
            return View();
        }

        public IActionResult BlogThumb()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadSlideBg(IFormFile GalleryImage)
        {
            var slideBg = new SlideBg();           
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                string folderName = "slider";
                if (GalleryImage != null)
                {
                    string newNumber = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToString();
                    newNumber = SD.RemoveSpecialCharacters(newNumber);
                    string fileName = "rendermentor-slide-" + newNumber;
                    ImageUploader.UploadImage(webRootPath, folderName, GalleryImage, slideBg.Image, fileName);
                    var extension = Path.GetExtension(GalleryImage.FileName);
                    slideBg.Image = @"\images\" + folderName + @"\" + fileName + extension;
                    var imagePath = Path.Combine(webRootPath, slideBg.Image.TrimStart('\\'));

                    var thumbnail = @"\images\" + folderName + @"\" + fileName + "-351x146" + extension;
                    var thumbPath = Path.Combine(webRootPath, thumbnail.TrimStart('\\'));

                    ImageProcessing.CropImage(imagePath, thumbPath, 351, 146);

                    if (System.IO.File.Exists(thumbPath))
                    {
                        slideBg.ImageSM = thumbnail;
                    }
                }

                _unitOfWork.SlideBg.Add(slideBg);
                _unitOfWork.Save();
            }

            return Json(new { success = true, message = "Görsel slayt galerisine eklendi." });
        }

        [HttpPost]
        public IActionResult UploadCourseCover(IFormFile GalleryImage)
        {
            var courseCover = new CourseCover();
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                string folderName = @"courses\course-cover";
                if (GalleryImage != null)
                {
                    string newNumber = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToString();
                    newNumber = SD.RemoveSpecialCharacters(newNumber);
                    string fileName = "rendermentor-course-" + newNumber;
                    ImageUploader.UploadImage(webRootPath, folderName, GalleryImage, courseCover.Image, fileName);
                    var extension = Path.GetExtension(GalleryImage.FileName);
                    courseCover.Image = @"\images\" + folderName + @"\" + fileName + extension;
                }

                _unitOfWork.CourseCover.Add(courseCover);
                _unitOfWork.Save();
            }

            return Json(new { success = true, message = "Görsel kurs kapak galerisine eklendi." });
        }

        [HttpPost]
        public IActionResult UploadBlogThumb(IFormFile GalleryImage)
        {
            var blogThumb = new BlogThumb();
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                string folderName = "blog";
                if (GalleryImage != null)
                {
                    string newNumber = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToString();
                    newNumber = SD.RemoveSpecialCharacters(newNumber);
                    string fileName = "rendermentor-blog-" + newNumber;
                    ImageUploader.UploadImage(webRootPath, folderName, GalleryImage, blogThumb.Image, fileName);
                    var extension = Path.GetExtension(GalleryImage.FileName);
                    blogThumb.Image = @"\images\" + folderName + @"\" + fileName + extension;
                    var imagePath = Path.Combine(webRootPath, blogThumb.Image.TrimStart('\\'));

                    var thumbnail = @"\images\" + folderName + @"\" + fileName + "-300x320" + extension;
                    var thumbPath = Path.Combine(webRootPath, thumbnail.TrimStart('\\'));
                    var thumbnailSm = @"\images\" + folderName + @"\" + fileName + "-203x100" + extension;
                    var thumbSmPath = Path.Combine(webRootPath, thumbnailSm.TrimStart('\\'));

                    ImageProcessing.CropImage(imagePath, thumbPath, 300, 320);

                    if (System.IO.File.Exists(thumbPath))
                    {
                        blogThumb.ImageSM = thumbnail;
                    }

                    ImageProcessing.CropImage(imagePath, thumbSmPath, 203, 100);

                    if (System.IO.File.Exists(thumbSmPath))
                    {
                        blogThumb.ImageXSM = thumbnailSm;
                    }
                }

                _unitOfWork.BlogThumb.Add(blogThumb);
                _unitOfWork.Save();
            }

            return Json(new { success = true, message = "Görsel blog galerisine eklendi." });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAllSlideBgs()
        {
            var allSlideBgs = _unitOfWork.SlideBg.GetAll();
            return Json(new { data = allSlideBgs });
        }

        [HttpGet]
        public IActionResult GetAllCourseCovers()
        {
            var allCourseCovers = _unitOfWork.CourseCover.GetAll();
            return Json(new { data = allCourseCovers });
        }

        [HttpGet]
        public IActionResult GetAllBlogThumbs()
        {
            var allBlogThumbs = _unitOfWork.BlogThumb.GetAll();
            return Json(new { data = allBlogThumbs });
        }

        [HttpDelete("Admin/MediaGallery/DeleteBulkSlideBgs/{toDeleteIds}")]
        public IActionResult DeleteBulkSlideBgs(string toDeleteIds)
        {
            string[] values = JsonConvert.DeserializeObject<string[]>(toDeleteIds);
            int[] idsArray = Array.ConvertAll(values, i => int.Parse(i));
            var toDelete = _unitOfWork.SlideBg.GetAll().Where(i => idsArray.Contains(i.Id)).ToList();
            if (toDelete.Count() == 0)
            {
                return Json(new { success = false, message = "Silme işlemi için görsel seçilmedi." });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string imagePath = "";
            string thumbPath = "";
            foreach (var item in toDelete)
            {
                if (item.Image != null)
                {
                    imagePath = Path.Combine(webRootPath, item.Image.TrimStart('\\'));
                }

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                if (item.ImageSM != null)
                {
                    thumbPath = Path.Combine(webRootPath, item.ImageSM.TrimStart('\\'));
                }
                if (System.IO.File.Exists(thumbPath))
                {
                    System.IO.File.Delete(thumbPath);
                }
            }

            _unitOfWork.SlideBg.RemoveRange(toDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Toplu silme işlemi başarılı." });
        }

        [HttpDelete("Admin/MediaGallery/DeleteBulkCourseCovers/{toDeleteIds}")]
        public IActionResult DeleteBulkCourseCovers(string toDeleteIds)
        {
            string[] values = JsonConvert.DeserializeObject<string[]>(toDeleteIds);
            int[] idsArray = Array.ConvertAll(values, i => int.Parse(i));
            var toDelete = _unitOfWork.CourseCover.GetAll().Where(i => idsArray.Contains(i.Id)).ToList();
            if (toDelete.Count() == 0)
            {
                return Json(new { success = false, message = "Silme işlemi için görsel seçilmedi." });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string imagePath = "";
            string thumbPath = "";
            foreach (var item in toDelete)
            {
                if (item.Image != null)
                {
                    imagePath = Path.Combine(webRootPath, item.Image.TrimStart('\\'));
                }

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                if (item.ImageSM != null)
                {
                    thumbPath = Path.Combine(webRootPath, item.ImageSM.TrimStart('\\'));
                }
                if (System.IO.File.Exists(thumbPath))
                {
                    System.IO.File.Delete(thumbPath);
                }
            }

            _unitOfWork.CourseCover.RemoveRange(toDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Toplu silme işlemi başarılı." });
        }

        [HttpDelete("Admin/MediaGallery/DeleteBulkBlogThumbs/{toDeleteIds}")]
        public IActionResult DeleteBulkBlogThumbs(string toDeleteIds)
        {
            string[] values = JsonConvert.DeserializeObject<string[]>(toDeleteIds);
            int[] idsArray = Array.ConvertAll(values, i => int.Parse(i));
            var toDelete = _unitOfWork.BlogThumb.GetAll().Where(i => idsArray.Contains(i.Id)).ToList();
            if (toDelete.Count() == 0)
            {
                return Json(new { success = false, message = "Silme işlemi için görsel seçilmedi." });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string imagePath = "";
            string thumbPath = "";
            string thumbSMPath = "";
            foreach (var item in toDelete)
            {
                if (item.Image != null)
                {
                    imagePath = Path.Combine(webRootPath, item.Image.TrimStart('\\'));
                }

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                if (item.ImageSM != null)
                {
                    thumbPath = Path.Combine(webRootPath, item.ImageSM.TrimStart('\\'));
                }
                if (System.IO.File.Exists(thumbPath))
                {
                    System.IO.File.Delete(thumbPath);
                }

                if (item.ImageSM != null)
                {
                    thumbSMPath = Path.Combine(webRootPath, item.ImageXSM.TrimStart('\\'));
                }
                if (System.IO.File.Exists(thumbSMPath))
                {
                    System.IO.File.Delete(thumbSMPath);
                }
            }

            _unitOfWork.BlogThumb.RemoveRange(toDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Toplu silme işlemi başarılı." });
        }

        public IActionResult DeleteSlideBg(int id)
        {
            var objFromDb = _unitOfWork.SlideBg.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string imagePath = "";
            string thumbPath = "";

            if (objFromDb.Image != null)
            {
                imagePath = Path.Combine(webRootPath, objFromDb.Image.TrimStart('\\'));
            }

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (objFromDb.ImageSM != null)
            {
                thumbPath = Path.Combine(webRootPath, objFromDb.ImageSM.TrimStart('\\'));
            }
            if (System.IO.File.Exists(thumbPath))
            {
                System.IO.File.Delete(thumbPath);
            }

            _unitOfWork.SlideBg.Remove(id);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        public IActionResult DeleteCourseCover(int id)
        {
            var objFromDb = _unitOfWork.CourseCover.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string imagePath = "";
            string thumbPath = "";

            if (objFromDb.Image != null)
            {
                imagePath = Path.Combine(webRootPath, objFromDb.Image.TrimStart('\\'));
            }

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (objFromDb.ImageSM != null)
            {
                thumbPath = Path.Combine(webRootPath, objFromDb.ImageSM.TrimStart('\\'));
            }
            if (System.IO.File.Exists(thumbPath))
            {
                System.IO.File.Delete(thumbPath);
            }

            _unitOfWork.CourseCover.Remove(id);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        public IActionResult DeleteBlogThumb(int id)
        {
            var objFromDb = _unitOfWork.BlogThumb.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string imagePath = "";
            string thumbPath = "";
            string thumbSMPath = "";

            if (objFromDb.Image != null)
            {
                imagePath = Path.Combine(webRootPath, objFromDb.Image.TrimStart('\\'));
            }

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (objFromDb.ImageSM != null)
            {
                thumbPath = Path.Combine(webRootPath, objFromDb.ImageSM.TrimStart('\\'));
            }
            if (System.IO.File.Exists(thumbPath))
            {
                System.IO.File.Delete(thumbPath);
            }

            if (objFromDb.ImageXSM != null)
            {
                thumbSMPath = Path.Combine(webRootPath, objFromDb.ImageXSM.TrimStart('\\'));
            }
            if (System.IO.File.Exists(thumbSMPath))
            {
                System.IO.File.Delete(thumbSMPath);
            }

            _unitOfWork.BlogThumb.Remove(id);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }
        #endregion
    }
}