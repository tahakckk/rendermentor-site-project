using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        private readonly ApplicationDbContext _db;

        public TeamRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Team team)
        {
            _db.Update(team);
        }

    }
}
