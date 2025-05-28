using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        private readonly ApplicationDbContext _db;

        public BlogRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Blog blog)
        {
            var objFromDb = _db.Blogs.FirstOrDefault(s => s.Id == blog.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = blog.Title;
                objFromDb.AuthorId = blog.AuthorId;
                objFromDb.CreateDate = blog.CreateDate;
                if (blog.Image != null)
                {
                    objFromDb.Image = blog.Image;
                }
                if (blog.Thumbnail != null)
                {
                    objFromDb.Thumbnail = blog.Thumbnail;
                }
                if (blog.ThumbnailSM != null)
                {
                    objFromDb.ThumbnailSM = blog.ThumbnailSM;
                }
                objFromDb.Desc = blog.Desc;
                objFromDb.MetaTitle = blog.MetaTitle;
                objFromDb.MetaDesc = blog.MetaDesc;
                objFromDb.SEOUrl = blog.SEOUrl;
            }
        }

    }
}
