using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class InstructorCategoryRepository : Repository<InstructorCategory>, IInstructorCategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public InstructorCategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InstructorCategory instructorCategory)
        {
            _db.Update(instructorCategory);
        }
    }
}
