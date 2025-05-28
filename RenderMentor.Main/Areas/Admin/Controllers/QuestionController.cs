using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class QuestionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public QuestionController(IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {            
            return View();
        }

        public IActionResult Answer(int id)
        {
            CultureInfo culture = new CultureInfo("tr-TR");
            var objFromDb = _unitOfWork.LectureQuestion.Get(id);
            var question = new LectureQuestionAdminVM() { 
                Id = objFromDb.Id,
                Date = objFromDb.Date.ToString("d/MMMM/yyy HH:mm", culture).Replace(".", " "),
                Question = objFromDb.Question,
                Answered = objFromDb.Answered,
                StudentName = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == objFromDb.UserId).Name,
                StudentEmail = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == objFromDb.UserId).Email,
            };
            var sectionId = _unitOfWork.Lecture.Get(objFromDb.LectureId).CourseSectionId;
            question.LectureName = _unitOfWork.Lecture.Get(objFromDb.LectureId).Title;
            question.CourseSectionName = _unitOfWork.CourseSection.Get(sectionId).Title;
            question.CourseId = _unitOfWork.CourseSection.Get(sectionId).CourseId;
            question.CourseName = _unitOfWork.Course.Get(question.CourseId).Title;
            question.LectureAnswers = _unitOfWork.LectureAnswer.GetAll().Where(i => i.LectureQuestionId == id)
                .Select(n => new LectureAnswerVM()
                {
                    Id = n.Id,
                    Date = n.Date.ToString("d/MMMM/yyy HH:mm", culture).Replace(".", " "),
                    Answer = n.Answer,
                    Name = _unitOfWork.ApplicationUser.GetFirstOrDefault(o => o.Id == n.UserId).Name,
                    IsInstructor = _userManager.IsInRoleAsync(_userManager.FindByIdAsync(n.UserId).Result, SD.Role_Instructor).Result ? true : false,
                }).ToList();
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnswerQuestion(int questionId, string answer)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var ans = new LectureAnswer()
            {
                Date = DateTime.Now,
                Answer = answer,
                UserId = claim.Value,
                LectureQuestionId = questionId,                
            };
            if (answer == null || String.IsNullOrEmpty(answer.Trim()))
            {
                return Json(new { success = false, message = "Lütfen cevap alanını doldurunuz." });
            }
            var question = _unitOfWork.LectureQuestion.Get(questionId);
            question.Answered = true;
            _unitOfWork.Save();

            int unanswered = _unitOfWork.LectureQuestion.GetAll(u => u.Answered == false).Count();
            HttpContext.Session.SetInt32(SD.PendingQuestions, unanswered);

            _unitOfWork.LectureAnswer.Add(ans);
            _unitOfWork.Save();

            var studentName = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == question.UserId).Name;
            var studentEmail = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == question.UserId).Email;
            var sectionId = _unitOfWork.Lecture.Get(question.LectureId).CourseSectionId;
            var lectureName = _unitOfWork.Lecture.Get(question.LectureId).Title;
            var courseSectionName = _unitOfWork.CourseSection.Get(sectionId).Title;
            var courseId = _unitOfWork.CourseSection.Get(sectionId).CourseId;
            var courseName = _unitOfWork.Course.Get(courseId).Title;

            _emailSender.SendEmailAsync(studentEmail, $"{courseName} kursundaki sorunuz eğitmen tarafından cevaplandı.", $"Sayın {studentName}, <strong>{courseName}</strong> adlı kursun <strong>{courseSectionName}</strong> bölümündeki <strong>{lectureName}</strong> dersi için sormuş olduğunuz soru eğitmeniniz tarafından cevaplandı.");

            var redirectUrl = $"{this.Request.Scheme}://{this.Request.Host}/Admin/Question/Index";
            return Json(new { success = true, Url = redirectUrl });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            CultureInfo culture = new CultureInfo("tr-TR");
            var allObj = _unitOfWork.LectureQuestion.GetAll()
                .Select(m => new LectureQuestionAdminVM() { 
                    Id = m.Id,
                    DateTime = m.Date,
                    CourseSectionId = _unitOfWork.Lecture.GetFirstOrDefault(n => n.Id == m.LectureId).CourseSectionId,
                    LectureName = _unitOfWork.Lecture.GetFirstOrDefault(n => n.Id == m.LectureId).Title,
                    StudentName = _unitOfWork.ApplicationUser.GetFirstOrDefault(n => n.Id == m.UserId).Name,
                    StudentEmail = _unitOfWork.ApplicationUser.GetFirstOrDefault(n => n.Id == m.UserId).Email,
                    Answered = m.Answered,
                    AnswerCount = _unitOfWork.LectureAnswer.GetAll().Where(n => n.LectureQuestionId == m.Id).Count()
                }).ToList();
            foreach (var item in allObj)
            {
                item.CourseSectionName = _unitOfWork.CourseSection.GetFirstOrDefault(i => i.Id == item.CourseSectionId).Title;
                item.CourseId = _unitOfWork.CourseSection.GetFirstOrDefault(i => i.Id == item.CourseSectionId).CourseId;
                item.CourseName = _unitOfWork.Course.GetFirstOrDefault(i => i.Id == item.CourseId).Title;
            }
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.LectureQuestion.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            _unitOfWork.LectureQuestion.Remove(id);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        [HttpDelete]
        public IActionResult DeleteAnswer(int id)
        {
            var objFromDb = _unitOfWork.LectureAnswer.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            _unitOfWork.LectureAnswer.Remove(id);
            var question = _unitOfWork.LectureQuestion.Get(objFromDb.LectureQuestionId);
            question.Answered = false;
            _unitOfWork.Save();
            var answered = _unitOfWork.LectureAnswer.GetAll().Where(i => i.LectureQuestionId == objFromDb.LectureQuestionId);
            foreach (var item in answered)
            {
                var ansbyIns = _userManager.IsInRoleAsync(_userManager.FindByIdAsync(item.UserId).Result, SD.Role_Instructor).Result;
                if (ansbyIns)
                {
                    question.Answered = true;
                }
            }
            if (question.Answered == false)
            {
                int unanswered = _unitOfWork.LectureQuestion.GetAll(u => u.Answered == false).Count();
                HttpContext.Session.SetInt32(SD.PendingQuestions, unanswered);
            }
            _unitOfWork.Save();

            var redirectUrl = $"{this.Request.Scheme}://{this.Request.Host}/Admin/Question/Index";
            return Json(new { success = true, Url = redirectUrl });
        }

        #endregion
    }
}