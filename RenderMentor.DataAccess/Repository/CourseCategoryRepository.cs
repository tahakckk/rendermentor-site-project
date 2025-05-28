using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class CourseCategoryRepository : Repository<CourseCategory>, ICourseCategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CourseCategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


		public void Update(CourseCategory courseCategory)
        {
            var objFromDb = _db.CourseCategories.FirstOrDefault(s => s.Id == courseCategory.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = courseCategory.Name;
                objFromDb.ShortDesc = courseCategory.ShortDesc;
                if (courseCategory.CoverImage != null)
                {
                    objFromDb.CoverImage = courseCategory.CoverImage;
                }
                objFromDb.PageTitle = courseCategory.PageTitle;
                objFromDb.PageDesc = courseCategory.PageDesc;
                if (courseCategory.PageBg != null)
                {
                    objFromDb.PageBg = courseCategory.PageBg;
                }
                objFromDb.MetaTitle = courseCategory.MetaTitle;
                objFromDb.MetaDesc = courseCategory.MetaDesc;
                objFromDb.SEOUrl = courseCategory.SEOUrl;
            }
        }

    }
}
