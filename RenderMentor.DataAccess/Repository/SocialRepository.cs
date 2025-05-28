using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class SocialRepository : Repository<Social>, ISocialRepository
    {
        private readonly ApplicationDbContext _db;

        public SocialRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Social social)
        {
            _db.Update(social);
        }

    }
}
