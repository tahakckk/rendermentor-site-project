using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class StudentCourseRepository : Repository<StudentCourse>, IStudentCourseRepository
    {
        private readonly ApplicationDbContext _db;

        public StudentCourseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(StudentCourse studentCourse)
        {
            _db.Update(studentCourse);
        }
    }
}
