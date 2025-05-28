using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class CourseGalleryRepository : Repository<CourseGallery>, ICourseGalleryRepository
    {
        private readonly ApplicationDbContext _db;

        public CourseGalleryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CourseGallery courseGallery)
        {
            var objFromDb = _db.CourseGalleries.FirstOrDefault(s => s.Id == courseGallery.Id);
            if (objFromDb != null)
            {
                objFromDb.Image = courseGallery.Image;
                objFromDb.CourseId = courseGallery.CourseId;
            }
        }
    }
}
