using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class MentorPageRepository : Repository<MentorPage>, IMentorPageRepository
    {
        private readonly ApplicationDbContext _db;

        public MentorPageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(MentorPage mentorPage)
        {
            _db.Update(mentorPage);
        }

    }
}
