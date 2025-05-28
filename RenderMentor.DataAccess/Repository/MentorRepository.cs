using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class MentorRepository : Repository<Mentor>, IMentorRepository
    {
        private readonly ApplicationDbContext _db;

        public MentorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Mentor mentor)
        {
            _db.Update(mentor);
        }

    }
}
