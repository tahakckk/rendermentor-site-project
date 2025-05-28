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
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    [Area("Admin")]
    public class ContractController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var contract = new Contract();
            if (id == null)
            {
                // Create
                return View(contract);
            }
            // Edit
            contract = _unitOfWork.Contract.Get(id.GetValueOrDefault());
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Contract contract)
        {
            var allObj = _unitOfWork.Contract.GetAll();
            var countObj = allObj.Count();

            // The instance of entity type cannot be tracked because another instance with the same key value for {'Id'} is already being tracked.
            // Update method in ContractRepository gives this error when an ObjFromDb used. In order to fix it, we remove _db.Update() from ContractRepository. We use another ObjFromDb in there and package contract values in there. That updates without _db.Update() method.
            var objFromDb = _unitOfWork.Contract.Get(contract.Id);
            if (ModelState.IsValid)
            {
                contract.SEOUrl = LinkConverter.CreateUrl(contract.Name);

                if (contract.Id == 0)
                {
                    contract.SEOUrl = LinkConverter.CreateUrl(contract.Name);
                }
                else if (objFromDb.SEOUrl == null || contract.Name != objFromDb.Name)
                {
                    contract.SEOUrl = LinkConverter.CreateUrl(contract.Name);
                }
                else
                {
                    contract.SEOUrl = objFromDb.SEOUrl;
                }

                if (contract.Id == 0)
                {
                    contract.ListOrder = countObj + 1;
                    _unitOfWork.Contract.Add(contract);
                }
                else
                {
                    _unitOfWork.Contract.Update(contract);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(contract);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Contract.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var allObj = _unitOfWork.Contract.GetAll();
            var objFromDb = _unitOfWork.Contract.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            _unitOfWork.Contract.Remove(objFromDb);
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
            var allObj = _unitOfWork.Contract.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        #endregion
    }
}