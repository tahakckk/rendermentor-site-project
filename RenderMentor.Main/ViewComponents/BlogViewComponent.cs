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
    public class BlogViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public BlogViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            CultureInfo culture = new CultureInfo("tr-TR");
            IEnumerable<BlogListVM> blogs = await _db.Blogs.Where(b => b.Published == true).OrderBy(b => b.ListOrder).Select(b => new BlogListVM()
            {
                Id = b.Id,
                Title = b.Title,
                CreateDate = b.CreateDate.ToString("d/MMMM/yyy", culture).Replace(".", " "),
                Image = b.Image,
                Thumbnail = b.Thumbnail,
                ThumbnailSM = b.ThumbnailSM,
                Desc = b.Desc,
                SEOUrl = b.SEOUrl,
                BlogThumbs = _db.BlogThumbs.Where(i => i.BlogId == b.Id).ToList(),
            }).ToListAsync();
            foreach (var item in blogs)
            {
                if (item.BlogThumbs.Count() > 0)
                {
                    Random r = new Random();
                    int total = item.BlogThumbs.Count();
                    int offset = r.Next(0, total);
                    var picked = item.BlogThumbs.Skip(offset).FirstOrDefault();
                    item.Image = picked.Image;
                    item.Thumbnail = picked.ImageSM;
                    item.ThumbnailSM = picked.ImageXSM;
                }
            }

            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, blogs);
            }
            return View(blogs);
        }
    }
}
