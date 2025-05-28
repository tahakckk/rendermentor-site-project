using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly ApplicationDbContext _db;

        public CourseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Course course)
        {
            var objFromDb = _db.Courses.FirstOrDefault(s => s.Id == course.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = course.Title;
                objFromDb.Price = course.Price;
                objFromDb.Published = course.Published;
                objFromDb.OnSale = course.OnSale;
                objFromDb.IsTrial = course.IsTrial;
                objFromDb.Description = course.Description;
                objFromDb.InstructorId = course.InstructorId;
                if (course.CoverImage != null)
                {
                    objFromDb.CoverImage = course.CoverImage;
                }
                objFromDb.Intro = course.Intro;
                objFromDb.CourseSections = course.CourseSections;
                objFromDb.MetaTitle = course.MetaTitle;
                objFromDb.MetaDesc = course.MetaDesc;
                objFromDb.CourseCategoryId = course.CourseCategoryId;
                objFromDb.CourseSections = course.CourseSections;
                objFromDb.SEOUrl = course.SEOUrl;
            }
        }

    }
}
