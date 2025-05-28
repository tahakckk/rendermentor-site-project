using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Base.Controllers
{
    [Area("Base")]
    public class CourseListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [Route("online-kurslar")]
        [Route("online-kurslar/{courseCategory}")]
        public IActionResult Index(string courseCategory)
        {
            var courseList = new CourseListVM();
            
            courseList.Courses = _unitOfWork.Course.GetAll(includeProperties: "CourseCategory,Instructor").Where(b => b.Published == true).OrderBy(c => c.ListOrder).AsQueryable();
            courseList.CourseList = _unitOfWork.CourseList.GetFirstOrDefault();
            
            foreach (var course in courseList.Courses)
            {
                course.TotalDuration = new TimeSpan(_unitOfWork.CourseSection.GetAll().Where(n => n.CourseId == course.Id)
                        .Select(n => new CourseSection()
                        {
                            TotalDuration = new TimeSpan(_unitOfWork.Lecture.GetAll().Where(x => x.CourseSectionId == n.Id).Sum(x => x.Duration.Ticks))
                        }).Sum(m => m.TotalDuration.Ticks));
                course.TotalLectures = _unitOfWork.CourseSection.GetAll().Where(n => n.CourseId == course.Id)
                        .Select(n => new CourseSection()
                        {
                            TotalLectures = _unitOfWork.Lecture.GetAll().Where(x => x.CourseSectionId == n.Id).Count()
                        }).Sum(x => x.TotalLectures);
                course.CourseCovers = _unitOfWork.CourseCover.GetAll().Where(i => i.CourseId == course.Id).ToList();
                if (course.CourseCovers.Count() > 0)
                {
                    Random r = new Random();
                    int total = course.CourseCovers.Count();
                    int offset = r.Next(0, total);
                    var picked = course.CourseCovers.Skip(offset).FirstOrDefault();
                    course.CoverImage = picked.Image;
                }
                if (course.Instructor == null)
                {
                    course.Instructor = new Instructor()
                    {
                        ApplicationUser = new ApplicationUser()
                        {
                            Name = "Evren Çavuşoğlu"
                        }
                    };
                }
                else
                {
                    course.Instructor = _unitOfWork.Instructor.GetAll().Where(i => i.Id == course.InstructorId)
                        .Select(i => new Instructor()
                        {
                            ApplicationUser = _unitOfWork.ApplicationUser.GetAll().Where(n => n.Id == i.UserId)
                                .Select(n => new ApplicationUser()
                                {
                                    Name = n.Name
                                }).FirstOrDefault()
                        }).FirstOrDefault();
                }
            }

            if (courseCategory != null)
            {
                courseList.CourseCategory = _unitOfWork.CourseCategory.GetFirstOrDefault(i => i.SEOUrl == courseCategory);
                int categoryId = courseList.CourseCategory.Id;
                courseList.Courses = courseList.Courses.Where(i => i.CourseCategoryId == categoryId);
                if (courseList.CourseCategory.MetaTitle != null)
                {
                    ViewData["Title"] = courseList.CourseCategory.MetaTitle;
                } else
                {
                    ViewData["Title"] = courseList.CourseCategory.Name + "- RenderMentor";
                }
                ViewData["Description"] = courseList.CourseCategory.MetaDesc;
            }
            else
            {
                ViewData["Title"] = "Tüm Online Kurslar - RenderMentor";
                ViewData["Description"] = "Bütün online kurslarımız hizmetinizdedir.";
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }

            return View(courseList);
        }

        [Route("online-kurslar/{courseCategory}/{SEOUrl}")]
        public IActionResult Details(string courseCategory, string SEOUrl)
        {
            var course = new CourseDetailVM();
            var getCourse = _unitOfWork.Course.GetFirstOrDefault(i => i.SEOUrl == SEOUrl);
            int? id = getCourse.Id;
            if (id == null)
            {
                // Create
                return View(course);
            }
            if (getCourse.Published == false)
            {
                return RedirectToAction(nameof(Index));
            }

            // Edit
            course.Id = getCourse.Id;
            course.Title = getCourse.Title;
            course.Price = getCourse.Price;
            course.Published = getCourse.Published;
            course.OnSale = getCourse.OnSale;
            course.Description = getCourse.Description;
            course.Intro = getCourse.Intro;
            course.MetaTitle = getCourse.MetaTitle;
            course.MetaDesc = getCourse.MetaDesc;
            course.CoverImage = getCourse.CoverImage;
            course.CourseListBg = _unitOfWork.CourseList.GetFirstOrDefault().PageBg;
            course.TotalLectures = _unitOfWork.Lecture.GetAll().Where(m => m.CourseId == id).Count();
            course.TotalDuration = new TimeSpan(_unitOfWork.Lecture.GetAll().Where(n => n.CourseId == id).Sum(n => n.Duration.Ticks));
            course.CourseSections = _unitOfWork.CourseSection.GetAll().Where(m => m.CourseId == id).OrderBy(m => m.ListOrder)
                .Select(m => new CourseSection()
                {
                    Id = m.Id,
                    Title = m.Title,
                    TotalDuration = new TimeSpan(_unitOfWork.Lecture.GetAll().Where(n => n.CourseSectionId == m.Id).Sum(n => n.Duration.Ticks)),
                    Lectures = _unitOfWork.Lecture.GetAll().Where(n => n.CourseSectionId == m.Id).OrderBy(n => n.ListOrder).ToList(),
                    TotalLectures = _unitOfWork.Lecture.GetAll().Where(n => n.CourseSectionId == m.Id).Count(),
                    ListOrder = m.ListOrder
                }).ToList();
            course.CourseGallery = _unitOfWork.CourseGallery.GetAll().Where(m => m.CourseId == id).OrderBy(m => m.ListOrder).ToList();
            course.CourseCategory = _unitOfWork.CourseCategory.GetFirstOrDefault(m => m.Id == getCourse.CourseCategoryId);
            course.Instructor = _unitOfWork.Instructor.GetFirstOrDefault(m => m.Id == getCourse.InstructorId);
            course.CourseReviews = _unitOfWork.CourseReview.GetAll(m => m.CourseId == id && m.Published == true, includeProperties: "ApplicationUser").OrderByDescending(i => i.Date);
            List<int> allRatings = new List<int>();
            foreach (var item in course.CourseReviews)
            {
                if (item.Rating != 0)
                {
                    allRatings.Add(item.Rating);
                }
            }
            if (allRatings.Count() > 0)
            {
                course.Rating = allRatings.Average();
            }

            // Check if the registered student has the course
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var isStudent = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;
                if (isStudent == SD.Role_User_Individual)
                {
                    var student = _unitOfWork.Student.GetFirstOrDefault(s => s.UserId == claim.Value);
                    if (student != null)
                    {
                        course.StudentCourseIDs = _unitOfWork.StudentCourse.GetAll().Where(s => s.StudentId == student.Id).Select(c => c.CourseId).ToArray();
                        var hasReview = _unitOfWork.CourseReview.GetFirstOrDefault(i => i.CourseId == id && i.UserId == claim.Value);
                        if (student.SubscribeActive)
                        {
                            course.StudentPackageActive = true;
                        }
                        if (hasReview != null)
                        {
                            course.StudentHasReview = true;
                        }
                    }
                    else
                    {
                        course.StudentCourseIDs = Array.Empty<int>();
                    }
                }
                else if (isStudent == SD.Role_Admin)
                {
                    // Admin kullanıcıları için özel işlemler
                    course.StudentPackageActive = true;
                    course.StudentCourseIDs = new[] { course.Id }; // Sadece mevcut kursun ID'sini içeren dizi
                    course.StudentHasReview = false;
                }
                else
                {
                    course.StudentCourseIDs = Array.Empty<int>();
                }
            }
            else
            {
                course.StudentCourseIDs = Array.Empty<int>();
            }

            if (course.Instructor == null)
            {
                course.Instructor = new Instructor()
                {
                    ApplicationUser = new ApplicationUser()
                    {
                        Name = "Evren Çavuşoğlu"
                    },
                    AvatarImage = "/images/placeholder/instructor-avatar-image.jpg",
                    Desc = "Demo description"
                };
                var instructorCategories = new List<InstructorCategory>();
                instructorCategories.Add(new InstructorCategory()
                {
                    CourseCategory = new CourseCategory()
                    {
                        Name = "Vray Dersleri"
                    }
                });
                course.InstructorCategories = instructorCategories;
            }
            else
            {
                course.Instructor = _unitOfWork.Instructor.GetAll().Where(i => i.Id == getCourse.InstructorId)
                    .Select(i => new Instructor()
                    {
                        ApplicationUser = _unitOfWork.ApplicationUser.GetAll().Where(n => n.Id == i.UserId)
                            .Select(n => new ApplicationUser()
                            {
                                Name = n.Name
                            }).FirstOrDefault(),
                        AvatarImage = i.AvatarImage,
                        Desc = i.Desc
                    }).FirstOrDefault();
                course.InstructorCategories = _unitOfWork.InstructorCategory.GetAll().Where(i => i.InstructorId == getCourse.InstructorId);
                if (course.Instructor.AvatarImage == null)
                {
                    course.Instructor.AvatarImage = "/images/placeholder/instructor-avatar-image.jpg";
                }
                foreach (var item in course.InstructorCategories)
                {
                    item.CourseCategory = _unitOfWork.CourseCategory.GetAll().Where(i => i.Id == item.CourseCategoryId)
                        .Select(i => new CourseCategory()
                        {
                            Name = i.Name
                        }).FirstOrDefault();
                }
            }

            if (course.MetaTitle != null)
            {
                ViewData["Title"] = course.MetaTitle;
            }
            else
            {
                ViewData["Title"] = course.CourseCategory.Name + " | RenderMentor";
            }
            if (course.MetaDesc != null)
            {
                ViewData["Description"] = course.MetaDesc;
            }
            else
            {
                ViewData["Description"] = course.CourseCategory.Name + " kategorisine ait online kursumuz " + course.TotalLectures + " ders içermektedir.";
            }
            ViewData["OgImage"] = course.CoverImage;

            course.ShoppingCart = new ShoppingCart()
            {
                Course = getCourse,
                CourseId = getCourse.Id
            };

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public IActionResult AddToCart(int courseId)
        //{
        //    var courseFromDb = _unitOfWork.Course.Get(courseId);
        //    var cartobject = new ShoppingCart()
        //    {
        //        Id = 0,
        //        CourseId = courseId,
        //        PackageId = 2
        //    };
        //    if (courseFromDb.OnSale && ModelState.IsValid)
        //    {
        //        // then we will add to cart
        //        var claimsIdentity = (ClaimsIdentity)User.Identity;
        //        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //        cartobject.ApplicationUserId = claim.Value;

        //        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
        //            u => u.ApplicationUserId == cartobject.ApplicationUserId && u.CourseId == cartobject.CourseId
        //            , includeProperties: "Course"
        //        );
        //        if (cartFromDb == null)
        //        {
        //            // no records exist in database for that product for that user
        //            _unitOfWork.ShoppingCart.Add(cartobject);
        //            _unitOfWork.Save();
        //        }
        //        var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == cartobject.ApplicationUserId).ToList().Count();

        //        // HttpContext.Session.SetObject(SD.ssShoppingCart, cartobject);
        //        HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

        //        var redirectUrl = Url.Action("Index", "Cart");
        //        return Json(new { success = true, Url = redirectUrl });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Sepete eklenemedi." });
        //    }
            
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult SendReview(int courseId, int rating, string comment)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var isStudent = claimsIdentity.FindFirst(ClaimTypes.Role).Value;
                if (isStudent == SD.Role_User_Individual)
                {
                    var hasReview = _unitOfWork.CourseReview.GetFirstOrDefault(i => i.CourseId == courseId && i.UserId == claim.Value);
                    if (hasReview == null)
                    {
                        var review = new CourseReview()
                        {
                            Date = DateTime.Now,
                            Rating = rating,
                            Comment = comment,
                            UserId = claim.Value,
                            CourseId = courseId
                        };
                        if (rating == 0 && (comment == null || String.IsNullOrEmpty(comment.Trim())))
                        {
                            return Json(new { success = false, message = "Lütfen puan veya yorum alanlarından birini doldurunuz." });
                        }
                        else
                        {
                            _unitOfWork.CourseReview.Add(review);
                            _unitOfWork.Save();
                        }
                    }
                }
            }
            
            var course = _unitOfWork.Course.GetFirstOrDefault(i => i.Id == courseId, includeProperties:"CourseCategory");
            var redirectUrl = $"{this.Request.Scheme}://{this.Request.Host}/online-kurslar/{course.CourseCategory.SEOUrl}/{course.SEOUrl}";
            //var redirectUrl = Url.RouteUrl("Details", new { CourseCategory = course.CourseCategory.SEOUrl, SEOUrl = course.SEOUrl });
            return Json(new { success = true, Url = redirectUrl });
        }
    }
}