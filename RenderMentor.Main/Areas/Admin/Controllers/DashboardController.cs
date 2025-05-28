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
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public DashboardVM DashboardVM { get; set; }

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            DashboardVM = new DashboardVM()
            {
                TotalStudents = _unitOfWork.Student.GetAll().Count(),
                TotalCourses = _unitOfWork.Course.GetAll().Count(),
                TotalOrders = _unitOfWork.OrderHeader.GetAll().Count(),
                TotalCompanies = _unitOfWork.Company.GetAll().Count(),
            };
            return View(DashboardVM);
        }

    }
}