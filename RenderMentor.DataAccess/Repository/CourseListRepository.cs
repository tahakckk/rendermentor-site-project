using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class CourseListRepository : Repository<CourseList>, ICourseListRepository
    {
        private readonly ApplicationDbContext _db;

        public CourseListRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(CourseList courseList)
        {
            _db.Update(courseList);
        }

    }
}
