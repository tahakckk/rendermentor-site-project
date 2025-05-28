using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    [Area("Admin")]
    public class TrialMemberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrialMemberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.TrialMember.GetAll();
            var trialMembers = new List<TrialMemberVM>();
            foreach (var item in allObj)
            {
                var package = new TrialMemberVM()
                {
                    StartDate = item.StartDate,
                    Student = _unitOfWork.Student.GetFirstOrDefault(i => i.Id == item.StudentId, includeProperties: "ApplicationUser")
                };
                package.StudentName = package.Student.ApplicationUser.Name;
                package.StudentEmail = package.Student.ApplicationUser.Email;
                if (item.IsActive)
                {
                    package.TrialStatus = "Aktif";
                }
                else
                {
                    package.TrialStatus = "Sona erdi";
                }
                trialMembers.Add(package);
            }
            return Json(new { data = trialMembers });
        }

        #endregion
    }
}