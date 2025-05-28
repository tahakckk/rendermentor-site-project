using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class BlogThumbRepository : Repository<BlogThumb>, IBlogThumbRepository
    {
        private readonly ApplicationDbContext _db;

        public BlogThumbRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BlogThumb blogThumb)
        {
            var objFromDb = _db.BlogThumbs.FirstOrDefault(s => s.Id == blogThumb.Id);
            if (objFromDb != null)
            {
                objFromDb.Image = blogThumb.Image;
                objFromDb.ImageSM = blogThumb.ImageSM;
                objFromDb.BlogId = blogThumb.BlogId;
            }
        }
    }
}
