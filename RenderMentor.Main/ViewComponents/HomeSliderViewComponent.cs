using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RenderMentor.ViewComponents
{
    public class HomeSliderViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public HomeSliderViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            var homeSliderOld = await _db.HomeSliders.OrderBy(i => i.ListOrder).ToListAsync();

            IEnumerable<HomeSlider> homeSlider = await _db.HomeSliders.OrderBy(h => h.ListOrder).Select(h => new HomeSlider()
            {
                Id = h.Id,
                Title = h.Title,
                Desc =  h.Desc,
                ButtonLink = h.ButtonLink,
                ButtonText = h.ButtonText,
                SlideBg = h.SlideBg,
                SlideBgs = _db.SlideBgs.Where(i => i.HomeSliderId == h.Id).ToList(),
            }).ToListAsync();
            foreach (var item in homeSlider)
            {
                if (item.SlideBgs.Count() > 0)
                {
                    Random r = new Random();
                    int total = item.SlideBgs.Count();
                    int offset = r.Next(0, total);
                    var picked = item.SlideBgs.Skip(offset).FirstOrDefault();
                    item.SlideBg = picked.Image;
                }
            }

            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, homeSlider);
            }
            return View(homeSlider);
        }
    }
}
