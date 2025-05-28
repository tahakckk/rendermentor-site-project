using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class CourseReviewRepository : Repository<CourseReview>, ICourseReviewRepository
    {
        private readonly ApplicationDbContext _db;

        public CourseReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(CourseReview courseReview)
        {
            _db.Update(courseReview);
        }

    }
}
