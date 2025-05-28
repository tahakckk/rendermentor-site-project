using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class CourseSectionRepository : Repository<CourseSection>, ICourseSectionRepository
    {
        private readonly ApplicationDbContext _db;

        public CourseSectionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CourseSection courseSection)
        {
            var objFromDb = _db.CourseSections.FirstOrDefault(s => s.Id == courseSection.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = courseSection.Title;
                objFromDb.TotalDuration = courseSection.TotalDuration;
                objFromDb.CourseId = courseSection.CourseId;
            }
        }
    }
}
