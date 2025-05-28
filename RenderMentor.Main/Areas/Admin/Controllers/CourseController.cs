using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using RenderMentor.Utility;
using RenderMentor.DataAccess.Data;
using RenderMentor.Utility.Helper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CourseController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            CourseList courseList = _unitOfWork.CourseList.GetFirstOrDefault();
            return View(courseList);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CourseList courseList, IFormFile PageBg)
        {
            // The instance of entity type cannot be tracked because another instance with the same key value for {'Id'} is already being tracked.
            // In order to fix the error above we added bool tracking = false in Repository (also in IRepository) GetFirstOrDefault() method. We also added a condition in the method; if tracking is false now we set query to query.AsNoTracking().
            var objFromDb = _unitOfWork.CourseList.GetFirstOrDefault(tracking: false);

            if (ModelState.IsValid)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "courses";
                if (PageBg != null)
                {
                    string fileName = objFromDb.SEOUrl + "-bg";
                    ImageUploader.UploadImage(webRoothPath, folderName, PageBg, objFromDb.PageBg, fileName);
                    var extension = Path.GetExtension(PageBg.FileName);
                    courseList.PageBg = @"\images\" + folderName + @"\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    courseList.PageBg = objFromDb.PageBg;
                }
                if (objFromDb.SEOUrl == null || objFromDb.Title != courseList.Title)
                {
                    courseList.SEOUrl = LinkConverter.CreateUrl(courseList.Title);
                } else
                {
                    courseList.SEOUrl = objFromDb.SEOUrl;
                }
                _unitOfWork.CourseList.Update(courseList);
                _unitOfWork.Save();

                return View(courseList);
            }
               
            return View(courseList);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Upsert(int? id)
        {
            CourseVM courseVM = new CourseVM()
            {
                Course = new Course(),
                CourseCategoryList = _unitOfWork.CourseCategory.GetAll().OrderBy(c => c.ListOrder).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CourseInstructorList = _unitOfWork.Instructor.GetAll().OrderBy(c => c.ListOrder).Select(i => new SelectListItem
                {
                    Text = _unitOfWork.ApplicationUser.GetAll().Where(m => m.Id == i.UserId).Select(m => m.Name).FirstOrDefault(),
                    Value = i.Id.ToString()
                })
            };            
            if (id == null)
            {
                // Create
                courseVM.Course.Published = true;
                courseVM.Course.OnSale = true;
                courseVM.Course.IsTrial = true;
                return View(courseVM);
            }
            // Edit
            courseVM.Course = _unitOfWork.Course.Get(id.GetValueOrDefault());
            courseVM.CategoryURL = _unitOfWork.CourseCategory.GetFirstOrDefault(i => i.Id == courseVM.Course.CourseCategoryId).SEOUrl;
            if (courseVM.Course == null)
            {
                return NotFound();
            }
            courseVM.Course.SEOUrl = LinkConverter.CreateUrl(courseVM.Course.Title);

            return View(courseVM);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CourseVM courseVM, IFormFile CoverImage)
        {
            var allObj = _unitOfWork.Course.GetAll();
            var objFromDb = _unitOfWork.Course.Get(courseVM.Course.Id);
            var countObj = allObj.Count();
            if (ModelState.IsValid)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "courses";
                if (CoverImage != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    if (courseVM.Course.Id != 0)
                    {
                        fileName = objFromDb.SEOUrl;
                        ImageUploader.UploadImage(webRoothPath, folderName, CoverImage, objFromDb.CoverImage, fileName);
                    } else
                    {
                        fileName = LinkConverter.CreateUrl(courseVM.Course.Title);
                        ImageUploader.UploadImage(webRoothPath, folderName, CoverImage, courseVM.Course.CoverImage, fileName);
                    }
                    var extension = Path.GetExtension(CoverImage.FileName);
                    courseVM.Course.CoverImage = @"\images\" + folderName + @"\" + fileName + extension;
                }
                else
                {
                    // update when they do not change the image
                    if (courseVM.Course.Id != 0)
                    {
                        courseVM.Course.CoverImage = objFromDb.CoverImage;
                    }
                }
                if (courseVM.Course.Id == 0)
                {
                    courseVM.Course.SEOUrl = LinkConverter.CreateUrl(courseVM.Course.Title);
                }
                else if (objFromDb.SEOUrl == null || courseVM.Course.Title != objFromDb.Title)
                {
                    courseVM.Course.SEOUrl = LinkConverter.CreateUrl(courseVM.Course.Title);
                }
                else
                {
                    courseVM.Course.SEOUrl = objFromDb.SEOUrl;
                }

                if (courseVM.Course.Id == 0)
                {
                    courseVM.Course.ListOrder = countObj + 1;
                    _unitOfWork.Course.Add(courseVM.Course);
                }
                else
                {                    
                    _unitOfWork.Course.Update(courseVM.Course);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                courseVM.CourseCategoryList = _unitOfWork.CourseCategory.GetAll().OrderBy(c => c.ListOrder).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                courseVM.CourseInstructorList = _unitOfWork.Instructor.GetAll().OrderBy(c => c.ListOrder).Select(i => new SelectListItem
                {
                    Text = _unitOfWork.ApplicationUser.GetAll().Where(m => m.Id == i.UserId).Select(m => m.Name).FirstOrDefault(),
                    Value = i.Id.ToString()
                });
                if (courseVM.Course.Id != 0)
                {
                    courseVM.Course = _unitOfWork.Course.Get(courseVM.Course.Id);
                }
            }

            return View(courseVM.Course);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSection(int secCourseId, string title)
        {
            if (!String.IsNullOrEmpty(title.Trim()))
            {
                var allObj = _unitOfWork.CourseSection.GetAll().Where(m => m.CourseId == secCourseId);
                var countObj = allObj.Count();
                var courseSection = new CourseSection()
                {
                    CourseId = secCourseId,
                    Title = title,
                    ListOrder = countObj + 1
                };

                _unitOfWork.CourseSection.Add(courseSection);
                _unitOfWork.Save();
            }
            else
            {
                ModelState.AddModelError("", "Başlık gerekli alandır. Lütfen doldurunuz.");
            }
            return RedirectToAction("Upsert", new { id = secCourseId });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLecture(int courseId, int courseSecId, string title, string video, TimeSpan lectureDuration, bool isPreview, string description, string mainDescription)
        {
            if (!String.IsNullOrEmpty(title.Trim()))
            {
                var allObj = _unitOfWork.Lecture.GetAll().Where(m => m.CourseSectionId == courseSecId);
                var countObj = allObj.Count();
                var lecture = new Lecture()
                {
                    CourseSectionId = courseSecId,
                    Title = title,
                    Video = video,
                    IsPreview = isPreview,
                    Description = description,
                    MainDescription = mainDescription,
                    Duration = lectureDuration,
                    ListOrder = countObj + 1,
                    CourseId = courseId
                };

                _unitOfWork.Lecture.Add(lecture);
                _unitOfWork.Save();
            }
            else
            {
                ModelState.AddModelError("", "Başlık gerekli alandır. Lütfen doldurunuz.");
            }
            return Json(new { success = true, message = "Ders bölüme eklendi." });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        [Route("findSection/{id}")]
        public IActionResult FindSection(int id)
        {
            var section = _unitOfWork.CourseSection.Get(id);
            return new JsonResult(section);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public IActionResult UpdateSection(int id, string title)
        {
            var section = _unitOfWork.CourseSection.Get(id);
            if (!String.IsNullOrEmpty(title.Trim()))
            {
                section.Title = title;
                _unitOfWork.CourseSection.Update(section);
                _unitOfWork.Save();
            }
            else
            {
                ModelState.AddModelError("", "Başlık gerekli alandır. Lütfen doldurunuz.");
            }

            return RedirectToAction("Upsert", new { id = section.CourseId });
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_User_Individual)]
        [HttpGet]
        [Route("findLecture/{courseId}/{lectureId}")]
        public IActionResult FindLecture(int courseId, int lectureId)
        {
            var lectureJson = new LectureSectionsVM()
            {
                Lecture = _unitOfWork.Lecture.Get(lectureId),
                CourseSections = _unitOfWork.CourseSection.GetAll().Where(n => n.CourseId == courseId).OrderBy(n => n.ListOrder).ToList()
            };
            // var sections = _unitOfWork.Course.Get(courseId).CourseSections;
            return new JsonResult(lectureJson);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public IActionResult UpdateLecture(int courseId, int lectureId, int courseSecId, string title, string video, TimeSpan lectureDuration, bool isPreview, string description, string mainDescription)
        {
            var lecture = _unitOfWork.Lecture.Get(lectureId);
            lecture.Title = title;
            lecture.Video = video;
            lecture.IsPreview = isPreview;
            lecture.Description = description;
            lecture.MainDescription = mainDescription;
            if (lecture.CourseSectionId != courseSecId)
            {
                var oldObj = _unitOfWork.Lecture.GetAll().Where(m => m.CourseSectionId == lecture.CourseSectionId);
                var allObj = _unitOfWork.Lecture.GetAll().Where(m => m.CourseSectionId == courseSecId);
                var countObj = allObj.Count();
                foreach (var item in oldObj)
                {
                    if (item.ListOrder > lecture.ListOrder)
                    {
                        item.ListOrder--;
                    }
                }
                lecture.ListOrder = countObj + 1;
            }
            lecture.CourseSectionId = courseSecId;
            lecture.Duration = lectureDuration;

            _unitOfWork.Lecture.Update(lecture);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Ders güncellendi." });
        }

        [HttpGet]
        public IActionResult FindPickedImages(int id)
        {
            var currentImages = _unitOfWork.CourseCover.GetAll().Where(i => i.CourseId == id).ToList();
            return new JsonResult(currentImages);
        }

        [HttpPost("Admin/Course/PickImages/{pickedImages}/{id}")]
        public IActionResult PickImages(string pickedImages, int id)
        {
            string[] values = JsonConvert.DeserializeObject<string[]>(pickedImages);
            int[] idsArray = Array.ConvertAll(values, i => int.Parse(i));
            var currentImages = _unitOfWork.CourseCover.GetAll().Where(i => i.CourseId == id).ToList();
            if (currentImages.Count() > 0)
            {
                var toDelete = currentImages.Where(i => !idsArray.Contains(i.Id)).ToList();
                foreach (var item in toDelete)
                {
                    item.CourseId = null;
                }
            }

            var toPick = _unitOfWork.CourseCover.GetAll().Where(i => idsArray.Contains(i.Id)).ToList();
            foreach (var item in toPick)
            {
                item.CourseId = id;
            }

            _unitOfWork.Save();
            return Json(new { success = true, message = "Toplu seçim işlemi başarılı." });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public IActionResult UploadGallery(IFormFile GalleryImage, int id)
        {
            var allObj = _unitOfWork.CourseGallery.GetAll().Where(m => m.CourseId == id);
            var thisCourse = _unitOfWork.Course.GetFirstOrDefault(i => i.Id == id);
            var countObj = allObj.Count();
            int newNumber = 0;
            if (countObj > 0)
            {
                newNumber = allObj.Last().Id + 1;
            }

            var courseGallery = new CourseGallery()
            {
                CourseId = id,
                ListOrder = countObj + 1
            };
            if (ModelState.IsValid)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = @"courses\course-gallery";
                if (GalleryImage != null)
                {
                    string fileName = LinkConverter.CreateUrl(thisCourse.Title) + "-";
                    if (countObj > 0)
                    {
                        fileName = fileName + newNumber;
                    }
                    else
                    {
                        fileName = fileName + 1;
                    }
                    ImageUploader.UploadImage(webRoothPath, folderName, GalleryImage, courseGallery.Image, fileName);
                    var extension = Path.GetExtension(GalleryImage.FileName);
                    courseGallery.Image = @"\images\" + folderName + @"\" + fileName + extension;
                }

                _unitOfWork.CourseGallery.Add(courseGallery);
                _unitOfWork.Save();
            }

            return Json(new { success = true, message = "Görsel galeriye eklendi." });
        }

        #region API CALLS

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var allCourses = _unitOfWork.Course.GetAll(includeProperties: "CourseCategory,Instructor");
            foreach (var course in allCourses)
            {
                if (course.Instructor == null)
                {
                    course.Instructor = new Instructor()
                    {
                        ApplicationUser = new ApplicationUser()
                        {
                            Name = ""
                        }
                    };
                }
                else
                {
                    course.Instructor = _unitOfWork.Instructor.GetAll().Where(i => i.Id == course.InstructorId)
                        .Select(i => new Instructor() { 
                            ApplicationUser = _unitOfWork.ApplicationUser.GetAll().Where(m => m.Id == i.UserId)
                                .Select(m => new ApplicationUser() { 
                                    Name = m.Name
                                }).FirstOrDefault()
                        }).FirstOrDefault();
                }
            }
            return Json(new { data = allCourses });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public IActionResult GetCourseSections(int id)
        {
            var allObj = _unitOfWork.CourseSection.GetAll().Where(m => m.CourseId == id)
                .Select(m => new CourseSection()
                {
                    Id = m.Id,
                    Title = m.Title,
                    ListOrder = m.ListOrder,
                    TotalDuration = m.TotalDuration,
                    CourseId = m.CourseId,
                    Lectures = _unitOfWork.Lecture.GetAll().Where(n => n.CourseSectionId == m.Id).OrderBy(n => n.ListOrder).ToList()
                });
            return Json(new { data = allObj });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public IActionResult GetSectionLectures(int id)
        {
            var allObj = _unitOfWork.Lecture.GetAll().Where(m => m.CourseSectionId == id);
            return Json(new { data = allObj });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public IActionResult GetCourseGallery(int id)
        {
            var allObj = _unitOfWork.CourseGallery.GetAll().Where(m => m.CourseId == id);
            return Json(new { data = allObj });
        }

        [HttpPost]
        public IActionResult PublishToggle([FromBody] int id)
        {
            var objFromDb = _unitOfWork.Course.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Yayınlama durumunda hata oluştu." });
            }
            if (objFromDb.Published == true)
            {
                // user is currently locked, we will unlock them
                objFromDb.Published = false;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Kurs yayından kaldırıldı." });
            }
            else
            {
                objFromDb.Published = true;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Kurs yayına alındı." });
            }
        }

        [HttpPost]
        public IActionResult SaleToggle([FromBody] int id)
        {
            var objFromDb = _unitOfWork.Course.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Satışa alma durumunda hata oluştu." });
            }
            if (objFromDb.OnSale == true)
            {
                // user is currently locked, we will unlock them
                objFromDb.OnSale = false;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Kurs satıştan kaldırıldı." });
            }
            else
            {
                objFromDb.OnSale = true;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Kurs satışa alındı." });
            }
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var allObj = _unitOfWork.Course.GetAll();
            var objFromDb = _unitOfWork.Course.Get(id);
            var subObj = _unitOfWork.CourseSection.GetAll().Where(m => m.CourseId == id);
            var subObj2 = _unitOfWork.CourseGallery.GetAll().Where(m => m.CourseId == id);
            var deepObj = _unitOfWork.Lecture.GetAll().Where(n => n.CourseId == id);


            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRoothPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRoothPath, objFromDb.CoverImage.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            if (subObj2.Count() != 0)
            {
                foreach (var item in subObj2)
                {
                    var galleryPath = Path.Combine(webRoothPath, item.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(galleryPath))
                    {
                        System.IO.File.Delete(galleryPath);
                    }
                }
                _unitOfWork.CourseGallery.RemoveRange(subObj2);
            }
            if (subObj.Count() != 0)
            {
                _unitOfWork.CourseSection.RemoveRange(subObj);
                if (deepObj.Count() != 0)
                {
                    _unitOfWork.Lecture.RemoveRange(deepObj);
                }
            }

            _unitOfWork.Course.Remove(objFromDb);
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

        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete("Admin/Course/DeleteSection/{courseId}/{id}")]
        public IActionResult DeleteSection(int courseId, int id)
        {
            var allObj = _unitOfWork.CourseSection.GetAll().Where(m => m.CourseId == courseId);
            var objFromDb = _unitOfWork.CourseSection.Get(id);
            var subObj = _unitOfWork.Lecture.GetAll().Where(n => n.CourseSectionId == id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            _unitOfWork.CourseSection.Remove(objFromDb);
            _unitOfWork.Lecture.RemoveRange(subObj);

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

        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete("Admin/Course/DeleteLecture/{courseSectionId}/{id}")]
        public IActionResult DeleteLecture(int courseSectionId, int id)
        {
            var allObj = _unitOfWork.Lecture.GetAll().Where(m => m.CourseSectionId == courseSectionId);
            var objFromDb = _unitOfWork.Lecture.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            _unitOfWork.Lecture.Remove(objFromDb);
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

        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete("Admin/Course/DeleteGalleryItem/{courseId}/{id}")]
        public IActionResult DeleteGalleryItem(int courseId, int id)
        {
            var allObj = _unitOfWork.CourseGallery.GetAll().Where(m => m.CourseId == courseId);
            var objFromDb = _unitOfWork.CourseGallery.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRoothPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRoothPath, objFromDb.Image.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.CourseGallery.Remove(objFromDb);
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

        [Authorize(Roles = SD.Role_Admin)]
        public void ListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.Course.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        [Authorize(Roles = SD.Role_Admin)]
        public void SectionListOrder(int id, int SectionId, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.CourseSection.GetAll().Where(m => m.CourseId == id);
            var changedObj = allObj.First(c => c.ListOrder == SectionId);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        [Authorize(Roles = SD.Role_Admin)]
        public void LectureListOrder(int id, int LectureId, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.Lecture.GetAll().Where(m => m.CourseSectionId == id);
            Lecture changedObj = allObj.First(c => c.ListOrder == LectureId);

            changedObj.ListOrder = toPosition;
            _unitOfWork.Save();
        }

        [Authorize(Roles = SD.Role_Admin)]
        public void GalleryListOrder(int id, int GalleryId, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.CourseGallery.GetAll().Where(m => m.CourseId == id);
            CourseGallery changedObj = allObj.First(c => c.ListOrder == GalleryId);

            changedObj.ListOrder = toPosition;
            _unitOfWork.Save();
        }

        #endregion
    }
}