using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class MembershipsRepository : Repository<Memberships>, IMembershipsRepository
    {
        private readonly ApplicationDbContext _db;

        public MembershipsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Memberships memberships)
        {
            _db.Update(memberships);
        }

    }
}
