using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Utility;
using static RenderMentor.Models.Social;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ContactController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            Contact contact = _unitOfWork.Contact.GetFirstOrDefault();            
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Contact.Update(contact);
                _unitOfWork.Save();

                return View(contact);
            }

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSocial(Socials socialName, string socialLink)
        {
            var allObj = _unitOfWork.Social.GetAll();
            var countObj = allObj.Count();
            var social = new Social()
            {
                Name = socialName,
                Link = socialLink,
                ListOrder = countObj + 1
            };

            _unitOfWork.Social.Add(social);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("findSocial/{id}")]
        public IActionResult FindSocial(int id)
        {
            var social = _unitOfWork.Social.Get(id);
            return new JsonResult(social);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSocial(int socialId, Socials socialName, string socialLink)
        {
            var social = _unitOfWork.Social.Get(socialId);
            var allObj = _unitOfWork.Social.GetAll();
            var countObj = allObj.Count();
            social.Name = socialName;
            social.Link = socialLink;
            
            _unitOfWork.Social.Update(social);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetSocials()
        {
            var allObj = _unitOfWork.Social.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult DeleteSocial(int id)
        {
            var allObj = _unitOfWork.Social.GetAll();
            var objFromDb = _unitOfWork.Social.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            _unitOfWork.Social.Remove(objFromDb);
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

        public void SocialListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.Social.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        #endregion
    }
}