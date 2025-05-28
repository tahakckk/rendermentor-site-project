using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class AboutRepository : Repository<About>, IAboutRepository
    {
        private readonly ApplicationDbContext _db;

        public AboutRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(About about)
        {
            _db.Update(about);
        }

    }
}
