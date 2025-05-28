using System;
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
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BlogController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("blog")]
        [Route("blog-sayfa{page}")]
        public IActionResult Index(int page = 1)
        {
            CultureInfo culture = new CultureInfo("tr-TR");
            List<BlogListVM> blogs = _unitOfWork.Blog.GetAll(includeProperties: "Author").Where(b => b.Published == true).OrderBy(b => b.ListOrder).Select(b => new BlogListVM()
            {
                Id = b.Id,
                Title = b.Title,
                CreateDate = b.CreateDate.ToString("d/MMMM/yyy", culture).Replace(".", " "),
                Author = b.Author == null ? new BlogAuthor() : new BlogAuthor()
                { 
                    Name = b.Author.Name,
                    Avatar = b.Author.Avatar,
                },
                Thumbnail = b.Thumbnail,
                Desc = b.Desc,
                SEOUrl = b.SEOUrl
            }).ToList();
            foreach (var blog in blogs)
            {
                blog.BlogThumbs = _unitOfWork.BlogThumb.GetAll().Where(t => t.BlogId == blog.Id).ToList();
                if (blog.BlogThumbs.Count() > 0)
                {
                    Random r = new Random();
                    int total = blog.BlogThumbs.Count();
                    int offset = r.Next(0, total);
                    var picked = blog.BlogThumbs.Skip(offset).FirstOrDefault();
                    blog.Thumbnail = picked.ImageSM;
                }
            }
            Pagination<BlogListVM> blogsPaging = Pagination<BlogListVM>.Create(blogs, page, 10);

            // View'daki kısım için custom tag helper yapalım.

            ViewData["Title"] = "Blog Makaleleri - RenderMentor";
            ViewData["Description"] = "Render ve modelleme hakkında faydalı bilgilere ulaşabileceğiniz blogumuz hizmetinizdedir.";

            return View(blogsPaging);
        }

        [Route("blog/{SEOUrl}")]
        public IActionResult Details(string SEOUrl)
        {
            var blog = new BlogDetailVM();
            var getBlog = _unitOfWork.Blog.GetFirstOrDefault(i => i.SEOUrl == SEOUrl, includeProperties: "Author");
            int? id = getBlog.Id;
            if (id == null)
            {
                // Create
                return View(blog);
            }
            if (getBlog.Published == false)
            {
                return RedirectToAction(nameof(Index));
            }

            // Edit
            CultureInfo culture = new CultureInfo("tr-TR");
            blog.Id = getBlog.Id;
            blog.Title = getBlog.Title;
            blog.CreateDate = getBlog.CreateDate.ToString("d/MMMM/yyy", culture).Replace(".", " ");
            blog.Author = getBlog.Author == null ? new BlogAuthor() : new BlogAuthor()
            { 
                Name = getBlog.Author.Name,
                Avatar = getBlog.Author.Avatar
            };
            blog.Desc = getBlog.Desc;
            blog.Image = getBlog.Image;
            blog.Published = getBlog.Published;
            blog.MetaTitle = getBlog.MetaTitle;
            blog.MetaDesc = getBlog.MetaDesc;

            blog.BlogThumbs = _unitOfWork.BlogThumb.GetAll().Where(i => i.BlogId == getBlog.Id).ToList();
            if (blog.BlogThumbs.Count() > 0)
            {
                Random r = new Random();
                int total = blog.BlogThumbs.Count();
                int offset = r.Next(0, total);
                var picked = blog.BlogThumbs.Skip(offset).FirstOrDefault();
                blog.Image = picked.Image;
            }

            blog.BlogAudios = _unitOfWork.BlogAudio.GetAll().Where(i => i.BlogId == getBlog.Id).OrderBy(i => i.ListOrder);

            if (blog.Published == false)
            {
                return RedirectToAction(nameof(Index));
            }
            
            if (blog.MetaTitle != null)
            {
                ViewData["Title"] = blog.MetaTitle;
            }
            else
            {
                ViewData["Title"] = blog.Title + " | RenderMentor";
            }
            if (blog.MetaDesc != null)
            {
                ViewData["Description"] = blog.MetaDesc;
            }
            else
            {
                ViewData["Description"] = blog.Title;
            }
            ViewData["OgImage"] = blog.Image;

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }
    }
}