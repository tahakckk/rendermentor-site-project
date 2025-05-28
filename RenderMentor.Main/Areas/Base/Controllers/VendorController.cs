using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Base.Controllers
{
    [Area("Base")]
    public class VendorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VendorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("bayilikler")]
        [Route("bayilik-sayfa{page}")]
        public IActionResult Index(int page = 1)
        {
            IEnumerable<Vendor> vendors = _unitOfWork.Vendor.GetAll().Where(b => b.Published == true).OrderBy(b => b.ListOrder).Select(b => new Vendor()
            {
                Id = b.Id,
                Name = b.Name,
                Thumbnail = b.Thumbnail,
                ShortDesc = b.ShortDesc,
                SEOUrl = b.SEOUrl
            });
            Pagination<Vendor> blogsPaging = Pagination<Vendor>.Create(vendors, page, 6);

            // View'daki kısım için custom tag helper yapalım.

            ViewData["Title"] = "Bayilikler - RenderMentor";
            ViewData["Description"] = "RenderMentor bayiliklerini bu sayfamızdan inceleyip ve bayilik sistemi hakkında detaylı bilgilere ulaşabilirsiniz.";

            return View(blogsPaging);
        }

        [Route("bayilikler/{SEOUrl}")]
        public IActionResult Details(string SEOUrl)
        {
            var vendor = new Vendor();
            var getVendor = _unitOfWork.Vendor.GetFirstOrDefault(i => i.SEOUrl == SEOUrl);
            int? id = getVendor.Id;
            if (id == null)
            {
                // Create
                return View(vendor);
            }
            if (getVendor.Published == false)
            {
                return RedirectToAction(nameof(Index));
            }

            // Edit
            vendor.Id = getVendor.Id;
            vendor.Name = getVendor.Name;
            vendor.Desc = getVendor.Desc;
            vendor.Image = getVendor.Image;
            vendor.Published = getVendor.Published;
            vendor.MetaTitle = getVendor.MetaTitle;
            vendor.MetaDesc = getVendor.MetaDesc;

            if (vendor.Published == false)
            {
                return RedirectToAction(nameof(Index));
            }
            
            if (vendor.MetaTitle != null)
            {
                ViewData["Title"] = vendor.MetaTitle;
            }
            else
            {
                ViewData["Title"] = vendor.Name + " | RenderMentor";
            }
            if (vendor.MetaDesc != null)
            {
                ViewData["Description"] = vendor.MetaDesc;
            }
            else
            {
                ViewData["Description"] = vendor.Name;
            }
            ViewData["OgImage"] = vendor.Image;

            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }
    }
}