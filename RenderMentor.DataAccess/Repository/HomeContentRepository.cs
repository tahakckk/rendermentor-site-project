using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class HomeContentRepository : Repository<HomeContent>, IHomeContentRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeContentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(HomeContent homeContent)
        {
            _db.Update(homeContent);
        }

    }
}
