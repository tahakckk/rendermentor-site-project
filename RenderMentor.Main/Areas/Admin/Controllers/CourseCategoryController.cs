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
    [Authorize(Roles = SD.Role_Admin)]
    [Area("Admin")]
    public class CourseCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CourseCategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var courseCategory = new CourseCategory();
            if (id == null)
            {
                // Create
                return View(courseCategory);
            }
            // Edit
            courseCategory = _unitOfWork.CourseCategory.Get(id.GetValueOrDefault());
            if (courseCategory == null)
            {
                return NotFound();
            }            

            return View(courseCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CourseCategory courseCategory, IFormFile CoverImage, IFormFile PageBg)
        {
            var allObj = _unitOfWork.CourseCategory.GetAll();
            var countObj = allObj.Count();
            var objFromDb = _unitOfWork.CourseCategory.Get(courseCategory.Id);
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                string folderName = "categories";
                if (CoverImage != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    if (courseCategory.Id != 0)
                    {
                        fileName = objFromDb.SEOUrl;
                        ImageUploader.UploadImage(webRootPath, folderName, CoverImage, objFromDb.CoverImage, fileName);
                    }
                    else
                    {
                        fileName = LinkConverter.CreateUrl(courseCategory.Name);
                        ImageUploader.UploadImage(webRootPath, folderName, CoverImage, courseCategory.CoverImage, fileName);
                    }
                    var extension = Path.GetExtension(CoverImage.FileName);
                    courseCategory.CoverImage = @"\images\" + folderName + @"\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    if (courseCategory.Id != 0)
                    {                        
                        courseCategory.CoverImage = objFromDb.CoverImage;
                    }
                }

                if (PageBg != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    if (courseCategory.Id != 0)
                    {
                        fileName = objFromDb.SEOUrl + "-bg";
                        ImageUploader.UploadImage(webRootPath, folderName, PageBg, objFromDb.PageBg, fileName);
                    }
                    else
                    {
                        fileName = LinkConverter.CreateUrl(courseCategory.Name + "-bg");
                        ImageUploader.UploadImage(webRootPath, folderName, PageBg, courseCategory.PageBg, fileName);
                    }
                    var extension = Path.GetExtension(PageBg.FileName);
                    courseCategory.PageBg = @"\images\" + folderName + @"\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    if (courseCategory.Id != 0)
                    {
                        courseCategory.PageBg = objFromDb.PageBg;
                    }
                }

                if (courseCategory.Id == 0)
                {
                    courseCategory.SEOUrl = LinkConverter.CreateUrl(courseCategory.Name);
                }
                else if (objFromDb.SEOUrl == null || courseCategory.Name != objFromDb.Name)
                {
                    courseCategory.SEOUrl = LinkConverter.CreateUrl(courseCategory.Name);
                }
                else
                {
                    courseCategory.SEOUrl = objFromDb.SEOUrl;
                }

                if (courseCategory.Id == 0)
                {
                    courseCategory.ListOrder = countObj + 1;
                    _unitOfWork.CourseCategory.Add(courseCategory);
                }
                else
                {
                    _unitOfWork.CourseCategory.Update(courseCategory);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            if (courseCategory.Id != 0)
            {
                courseCategory = _unitOfWork.CourseCategory.Get(courseCategory.Id);
            }

            return View(courseCategory);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.CourseCategory.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var allObj = _unitOfWork.CourseCategory.GetAll();
            var objFromDb = _unitOfWork.CourseCategory.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string imagePath = "";
            if (objFromDb.CoverImage != null)
            {
                imagePath = Path.Combine(webRootPath, objFromDb.CoverImage.TrimStart('\\'));
            }
            
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            string imagePath2 = "";
            if (objFromDb.PageBg != null)
            {
                imagePath2 = Path.Combine(webRootPath, objFromDb.PageBg.TrimStart('\\'));
            }
            
            if (System.IO.File.Exists(imagePath2))
            {
                System.IO.File.Delete(imagePath2);
            }
            _unitOfWork.CourseCategory.Remove(objFromDb);
            foreach (var item in allObj)
            {
                if (item.ListOrder > objFromDb.ListOrder)
                {
                    item.ListOrder --;
                }
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        public void ListOrder(int Id, int fromPosition, int toPosition)
        {            
            var allObj = _unitOfWork.CourseCategory.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);
            
            changedObj.ListOrder = toPosition;
            
            _unitOfWork.Save();
        }

        #endregion
    }
}