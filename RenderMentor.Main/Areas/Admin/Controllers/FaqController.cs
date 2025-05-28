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
    public class FaqController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;        

        public FaqController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var faq = new Faq();
            if (id == null)
            {
                // Create
                return View(faq);
            }
            // Edit
            faq = _unitOfWork.Faq.Get(id.GetValueOrDefault());
            if (faq == null)
            {
                return NotFound();
            }

            return View(faq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Faq faq)
        {
            var allObj = _unitOfWork.Faq.GetAll();
            var countObj = allObj.Count();

            // The instance of entity type cannot be tracked because another instance with the same key value for {'Id'} is already being tracked.
            // Update method in FaqRepository gives this error when an ObjFromDb used. In order to fix it, we remove _db.Update() from FaqRepository. We use another ObjFromDb in there and package faq values in there. That updates without _db.Update() method.
            var objFromDb = _unitOfWork.Faq.Get(faq.Id);
            if (ModelState.IsValid)
            {
                if (faq.Id == 0)
                {
                    faq.ListOrder = countObj + 1;
                    _unitOfWork.Faq.Add(faq);
                }
                else
                {
                    _unitOfWork.Faq.Update(faq);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(faq);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Faq.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var allObj = _unitOfWork.Faq.GetAll();
            var objFromDb = _unitOfWork.Faq.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            _unitOfWork.Faq.Remove(objFromDb);
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
            var allObj = _unitOfWork.Faq.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        #endregion

    }
}