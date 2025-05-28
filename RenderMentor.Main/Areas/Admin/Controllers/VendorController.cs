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
    public class VendorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public VendorController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
            Vendor vendor = new Vendor();
            if (id == null)
            {
                // Create
                return View(vendor);
            }
            // Edit
            vendor = _unitOfWork.Vendor.Get(id.GetValueOrDefault());
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Vendor vendor, IFormFile VendorImage)
        {
            var allObj = _unitOfWork.Vendor.GetAll();
            var countObj = allObj.Count();
            var objFromDb = _unitOfWork.Vendor.GetFirstOrDefault(i => i.Id == vendor.Id, tracking: false);
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                string folderName = "vendors";
                if (VendorImage != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    if (vendor.Id != 0)
                    {
                        fileName = objFromDb.SEOUrl;
                        ImageUploader.UploadImage(webRootPath, folderName, VendorImage, vendor.Image, fileName);
                    }
                    else
                    {
                        fileName = LinkConverter.CreateUrl(vendor.Name);
                        ImageUploader.UploadImage(webRootPath, folderName, VendorImage, vendor.Image, fileName);
                    }
                    var extension = Path.GetExtension(VendorImage.FileName);
                    string noCache = "?v=" + RandomGenerator.GenerateRandomNo(100, 999);

                    vendor.Image = @"\images\" + folderName + @"\" + fileName + extension;
                    var imagePath = Path.Combine(webRootPath, vendor.Image.TrimStart('\\'));
                    vendor.Image = vendor.Image + noCache;
                    var thumbnail = @"\images\" + folderName + @"\" + fileName + "-384x225" + extension;
                    var thumbPath = Path.Combine(webRootPath, thumbnail.TrimStart('\\'));

                    if (System.IO.File.Exists(thumbPath))
                    {
                        System.IO.File.Delete(thumbPath);
                    }

                    ImageProcessing.CropImage(imagePath, thumbPath, 384, 225);

                    if (System.IO.File.Exists(thumbPath))
                    {
                        vendor.Thumbnail = thumbnail + noCache;
                    }

                }
                else
                {
                    // update when they do not change the image
                    if (vendor.Id != 0)
                    {
                        vendor.Image = objFromDb.Image;
                    }
                }

                if (vendor.Id == 0)
                {
                    vendor.SEOUrl = LinkConverter.CreateUrl(vendor.Name);
                }
                else if (objFromDb.SEOUrl == null || vendor.Name != objFromDb.Name)
                {
                    vendor.SEOUrl = LinkConverter.CreateUrl(vendor.Name);
                }
                else
                {
                    vendor.SEOUrl = objFromDb.SEOUrl;
                }

                if (vendor.Id == 0)
                {
                    vendor.Published = true;
                    // blog date should not be set automatically when it's changeable
                    //blogVM.Blog.CreateDate = DateTime.Now;
                    vendor.ListOrder = 1;
                    foreach (var item in allObj)
                    {
                        item.ListOrder++;
                    }
                    _unitOfWork.Vendor.Add(vendor);
                }
                else
                {
                    _unitOfWork.Vendor.Update(vendor);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            if (vendor.Id != 0)
            {
                vendor = _unitOfWork.Vendor.Get(vendor.Id);
            }

            return View(vendor);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Vendor.GetAll();
            return Json(new { data = allObj });
        }

        [HttpPost]
        public IActionResult PublishToggle([FromBody] int id)
        {
            var objFromDb = _unitOfWork.Vendor.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Yayınlama durumunda hata oluştu." });
            }
            if (objFromDb.Published == true)
            {
                // user is currently locked, we will unlock them
                objFromDb.Published = false;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Blog yayından kaldırıldı." });
            }
            else
            {
                objFromDb.Published = true;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Blog yayına alındı." });
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var allObj = _unitOfWork.Vendor.GetAll();
            var objFromDb = _unitOfWork.Vendor.Get(id);
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

                // The code below removes noCache (?v=xxx). Because the suffix to prevent browser caching of the value in db won't match with actual image path.
                imagePath = imagePath.Remove(imagePath.Length - 6, 6);
            }
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (objFromDb.Thumbnail != null)
            {
                thumbPath = Path.Combine(webRootPath, objFromDb.Thumbnail.TrimStart('\\'));
                thumbPath = thumbPath.Remove(thumbPath.Length - 6, 6);
            }
            if (System.IO.File.Exists(thumbPath))
            {
                System.IO.File.Delete(thumbPath);
            }

            _unitOfWork.Vendor.Remove(id);
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
            var allObj = _unitOfWork.Vendor.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        #endregion
    }
}