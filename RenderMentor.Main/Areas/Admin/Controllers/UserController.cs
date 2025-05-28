using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdminStudentVM> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            ILogger<AdminStudentVM> logger,
            IUnitOfWork unitOfWork)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(string Id)
        {
            var student = new AdminStudentVM();
            
            student.CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            if (String.IsNullOrEmpty(Id))
            {
                return View(student);
            }
            // Edit
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == Id);
            var userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == user.Id);
            student.Id = user.Id;
            student.Name = user.Name;
            student.Email = user.Email;
            student.EmailConfirmed = user.EmailConfirmed;
            student.PhoneNumber = user.PhoneNumber;
            student.Gender = user.Gender;
            student.SubscribeActive = userStudent.SubscribeActive;
            if (userStudent.SubscribeActive)
            {
                student.SubscribePackageName = _unitOfWork.Subscription.GetFirstOrDefault().Name;
                student.SubscribeStartDate = userStudent.SubscribeStartDate;
                student.SubscribeExpireDate = userStudent.SubscribeExpireDate;
            }
            student.CompanyId = user.CompanyId;
            student.Role = user.Role;
            student.AllCourses = _unitOfWork.Course.GetAll().OrderBy(c => c.ListOrder).ToList();
            student.SelectedCourses = _unitOfWork.StudentCourse.GetAll().Where(i => i.StudentId == userStudent.Id).ToList();
            
            if (user == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(AdminStudentVM student)
        {
            if (ModelState.IsValid)
            {                
                if (String.IsNullOrEmpty(student.Id) || student.Id == "0")
                {
                    var user = new ApplicationUser()
                    {
                        Name = student.Name,
                        Email = student.Email,
                        UserName = student.Email,
                        EmailConfirmed = true,
                        PhoneNumber = student.PhoneNumber,
                        Gender = student.Gender,
                        CompanyId = student.CompanyId,
                        Role = SD.Role_User_Individual,
                        CreateDate = DateTime.Now
                    };
                    var createUser = await _userManager.CreateAsync(user, student.Password);
                    if (createUser.Succeeded)
                    {
                        _logger.LogInformation("Admin created a new user account with password.");
                        await _userManager.AddToRoleAsync(user, SD.Role_User_Individual);
                        if (student.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_User_Company);
                            user.Role = SD.Role_User_Company;
                        }
                        // Models.Student is necessary to pass it as class. Because it conflicts with the area name Student. It's processed as a namespace. Therefore it gives the error 'Student' is a namespace but it's used like a type.
                        var userStudent = new Models.Student() { 
                            UserId = user.Id
                        };
                        _unitOfWork.Student.Add(userStudent);
                        _unitOfWork.Save();
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    var thisUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == student.Id);
                    thisUser.Name = student.Name;
                    thisUser.Gender = student.Gender;
                    thisUser.EmailConfirmed = student.EmailConfirmed;
                    if (student.Email != thisUser.Email)
                    {
                        var code = await _userManager.GenerateChangeEmailTokenAsync(thisUser, student.Email);
                        await _userManager.ChangeEmailAsync(thisUser, student.Email, code);
                        await _userManager.SetUserNameAsync(thisUser, student.Email);
                        thisUser.EmailConfirmed = true;
                    }
                    if (student.CompanyId != thisUser.CompanyId)
                    {
                        if (student.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(thisUser, SD.Role_User_Company);
                            thisUser.CompanyId = student.CompanyId;
                            thisUser.Role = SD.Role_User_Company;
                        }
                        else
                        {
                            await _userManager.RemoveFromRoleAsync(thisUser, SD.Role_User_Company);
                            thisUser.CompanyId = null;
                            thisUser.Role = SD.Role_User_Individual;
                        }
                    }
                    if (!String.IsNullOrEmpty(student.Password))
                    {
                        String hashedNewPassword = _userManager.PasswordHasher.HashPassword(thisUser, student.Password);
                        thisUser.PasswordHash = hashedNewPassword;
                    }
                    _db.Update(thisUser);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(student);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public IActionResult StartPackage(string id)
        {
            var userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == id);

            if (userStudent.SubscribeActive)
            {
                return Json(new { success = false, message = "Öğrencinin aktif bir üyelik pakedi zaten bulunmaktadır." });
            }

            var subscription = _unitOfWork.Subscription.GetFirstOrDefault();

            userStudent.SubscribeActive = true;
            userStudent.SubscribeStarted = true;
            userStudent.SubscribeStartDate = DateTime.Now;
            userStudent.SubscribeExpireDate = userStudent.SubscribeStartDate.AddDays(subscription.ExpirationDays);

            _unitOfWork.Save();

            return Json(new { success = true, message = $"Öğrenci için {subscription.Name} pakedini aktive ettiniz." });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {

            var studentRole = _db.Roles.FirstOrDefault(a => a.Name == SD.Role_User_Individual).Id;
            var studentIds = _db.UserRoles.Where(a => a.RoleId == studentRole).Select(b => b.UserId).Distinct().ToList();
            var userList = _db.ApplicationUsers.Where(a => studentIds.Any(c => c == a.Id)).Include(u => u.Company).ToList();
            foreach (var user in userList)
            {
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult StudentCourses(int[] studentCourse, string userId)
        {

            var userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == userId);
            userStudent.StudentCourses = _unitOfWork.StudentCourse.GetAll(i => i.StudentId == userStudent.Id).ToList();
            var existingCourses = userStudent.StudentCourses.Select(i => i.CourseId).ToList();
            var toAdd = studentCourse.Except(existingCourses).ToList();
            var toRemove = existingCourses.Except(studentCourse).ToList();
            userStudent.StudentCourses = userStudent.StudentCourses.Where(i => !toRemove.Contains(i.CourseId) || i.isPaid == true).ToList();
            foreach (var item in toAdd)
            {
                userStudent.StudentCourses.Add(new StudentCourse()
                {
                    StudentId = userStudent.Id,
                    CourseId = item
                });
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Öğrenci kursları düzenlendi." });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Kullanıcı kilitleme işlemi esnasında bir hata oluştu." });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // user is currently locked, we will unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Kullanıcı kilitleme işlemi başarılı.." });
        }

        #endregion
    }
}