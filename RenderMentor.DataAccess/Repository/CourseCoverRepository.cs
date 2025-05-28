using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class CourseCoverRepository : Repository<CourseCover>, ICourseCoverRepository
    {
        private readonly ApplicationDbContext _db;

        public CourseCoverRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CourseCover courseCover)
        {
            var objFromDb = _db.CourseCovers.FirstOrDefault(s => s.Id == courseCover.Id);
            if (objFromDb != null)
            {
                objFromDb.Image = courseCover.Image;
                objFromDb.CourseId = courseCover.CourseId;
            }
        }
    }
}
