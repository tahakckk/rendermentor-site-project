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
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class MembershipsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MembershipsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            Memberships memberships = _unitOfWork.Memberships.GetFirstOrDefault();
            if (memberships == null)
            {
                memberships = new Memberships();
            }
            return View(memberships);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Memberships memberships)
        {
            var objFromDb = _unitOfWork.Memberships.GetFirstOrDefault(tracking: false);

            if (ModelState.IsValid)
            {
                
                _unitOfWork.Memberships.Update(memberships);
                _unitOfWork.Save();

                return View(memberships);
            }

            return View(memberships);
        }

        #region API CALLS

        #endregion
    }
}