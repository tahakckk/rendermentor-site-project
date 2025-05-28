using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    [Area("Admin")]
    public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdminStudentVM> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public InstructorController(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            ILogger<AdminStudentVM> logger,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var instructor = new AdminInstructorVM();
            if (id == 0 || id == null)
            {
                return View(instructor);
            }

            // Edit
            var userInstructor = _unitOfWork.Instructor.GetFirstOrDefault(m => m.Id == id);
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userInstructor.UserId);
            instructor.Id = userInstructor.Id;
            instructor.UserId = user.Id;
            instructor.Name = user.Name;
            instructor.Email = user.Email;
            instructor.EmailConfirmed = user.EmailConfirmed;
            instructor.PhoneNumber = user.PhoneNumber;
            instructor.Gender = user.Gender;
            instructor.Role = user.Role;
            instructor.Id = userInstructor.Id;
            instructor.Desc = userInstructor.Desc;
            instructor.AvatarImage = userInstructor.AvatarImage;
            instructor.AllCategories = _unitOfWork.CourseCategory.GetAll().OrderBy(c => c.ListOrder).ToList();
            instructor.SelectedCategories = _unitOfWork.InstructorCategory.GetAll().Where(i => i.InstructorId == userInstructor.Id).Select(i => i.CourseCategoryId).ToList();
            instructor.IsAdmin = userInstructor.IsAdmin;

            if (user == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(AdminInstructorVM instructor, IFormFile AvatarImage)
        {
            var allObj = _unitOfWork.Instructor.GetAll();

            // It's need if uploaded images has not changed. Instead of instructor.Image we need to use objFromDb.Image. Because otherwise the image fields in the table will be nulled as images in instructor Object arrive empty from View.
            var objFromDb = _unitOfWork.Instructor.Get(instructor.Id);

            var countObj = allObj.Count();            

            if (ModelState.IsValid)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "instructors";
                if (instructor.Id == 0)
                {
                    var user = new ApplicationUser()
                    {
                        Name = instructor.Name,
                        Email = instructor.Email,
                        UserName = instructor.Email,
                        EmailConfirmed = true,
                        PhoneNumber = instructor.PhoneNumber,
                        Gender = instructor.Gender,
                        Role = SD.Role_Instructor
                    };
                    var createUser = await _userManager.CreateAsync(user, instructor.Password);
                    if (createUser.Succeeded)
                    {
                        _logger.LogInformation("Admin created a new instructor account with password.");
                        await _userManager.AddToRoleAsync(user, SD.Role_Instructor);
                        var userInstructor = new Instructor()
                        {
                            UserId = user.Id,
                            Desc = instructor.Desc,
                            ListOrder = countObj + 1
                        };
                        
                        if (AvatarImage != null)
                        {
                            string fileName = LinkConverter.CreateUrl(user.Name);
                            ImageUploader.UploadImage(webRoothPath, folderName, AvatarImage, objFromDb.AvatarImage, fileName);
                            var extension = Path.GetExtension(AvatarImage.FileName);
                            userInstructor.AvatarImage = @"\images\" + folderName + @"\" + fileName + extension;
                        }

                        _unitOfWork.Instructor.Add(userInstructor);
                        _unitOfWork.Save();
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    var thisUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == instructor.UserId);
                    var userInstructor = _unitOfWork.Instructor.Get(instructor.Id);
                    thisUser.Name = instructor.Name;
                    thisUser.Gender = instructor.Gender;
                    thisUser.PhoneNumber = instructor.PhoneNumber;
                    thisUser.EmailConfirmed = instructor.EmailConfirmed;
                    userInstructor.Desc = instructor.Desc;
                    if (AvatarImage != null)
                    {
                        string fileName = LinkConverter.CreateUrl(thisUser.Name);
                        ImageUploader.UploadImage(webRoothPath, folderName, AvatarImage, objFromDb.AvatarImage, fileName);
                        var extension = Path.GetExtension(AvatarImage.FileName);

                        string noCache = "?v=" + RandomGenerator.GenerateRandomNo(100, 999);

                        userInstructor.AvatarImage = @"\images\" + folderName + @"\" + fileName + extension + noCache;
                    }
                    else
                    {
                        userInstructor.AvatarImage = objFromDb.AvatarImage;
                    }
                    if (instructor.Email != thisUser.Email)
                    {
                        var code = await _userManager.GenerateChangeEmailTokenAsync(thisUser, instructor.Email);
                        await _userManager.ChangeEmailAsync(thisUser, instructor.Email, code);
                        await _userManager.SetUserNameAsync(thisUser, instructor.Email);
                        thisUser.EmailConfirmed = true;
                    }
                    if (!String.IsNullOrEmpty(instructor.Password))
                    {
                        String hashedNewPassword = _userManager.PasswordHasher.HashPassword(thisUser, instructor.Password);
                        thisUser.PasswordHash = hashedNewPassword;
                    }
                    _db.Update(thisUser);
                    _db.Update(userInstructor);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(instructor);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var instructorList = _unitOfWork.Instructor.GetAll(includeProperties: "ApplicationUser");
            return Json(new { data = instructorList });
        }

        [HttpPost]
        public IActionResult InstructorCategories(int[] instructorCategory, string userId)
        {
            var userInstructor = _unitOfWork.Instructor.GetFirstOrDefault(i => i.UserId == userId);
            userInstructor.InstructorCategories = _unitOfWork.InstructorCategory.GetAll(i => i.InstructorId == userInstructor.Id).ToList();
            var existingCategories = userInstructor.InstructorCategories.Select(i => i.CourseCategoryId).ToList();
            var toAdd = instructorCategory.Except(existingCategories).ToList();
            var toRemove = existingCategories.Except(instructorCategory).ToList();
            userInstructor.InstructorCategories = userInstructor.InstructorCategories.Where(i => !toRemove.Contains(i.CourseCategoryId)).ToList();
            foreach (var item in toAdd)
            {
                userInstructor.InstructorCategories.Add(new InstructorCategory()
                {
                    InstructorId = userInstructor.Id,
                    CourseCategoryId = item,
                });
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Eğitmen kategorileri düzenlendi." });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] int id)
        {
            var objFromDb = _db.Instructors.FirstOrDefault(u => u.Id == id);
            var thisUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == objFromDb.UserId);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (thisUser.LockoutEnd != null && thisUser.LockoutEnd > DateTime.Now)
            {
                // user is currently locked, we will unlock them
                thisUser.LockoutEnd = DateTime.Now;
            }
            else
            {
                thisUser.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Kullanıcı kilitleme işlemi başarılı." });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var allObj = _unitOfWork.Instructor.GetAll();
            var objFromDb = _unitOfWork.Instructor.Get(id);
            var thisUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == objFromDb.UserId);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRoothPath = _hostEnvironment.WebRootPath;
            if (objFromDb.AvatarImage != null)
            {
                var imagePath = Path.Combine(webRoothPath, objFromDb.AvatarImage.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _unitOfWork.Instructor.Remove(objFromDb);
            _unitOfWork.ApplicationUser.Remove(thisUser);
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

        public void ListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.Instructor.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        #endregion
    }
}