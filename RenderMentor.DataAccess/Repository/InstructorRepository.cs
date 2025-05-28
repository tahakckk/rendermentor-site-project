using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class InstructorRepository : Repository<Instructor>, IInstructorRepository
    {
        private readonly ApplicationDbContext _db;

        public InstructorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Instructor instructor)
        {
            _db.Update(instructor);
        }
    }
}
