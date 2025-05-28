using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class DrawParticipantRepository : Repository<DrawParticipant>, IDrawParticipantRepository
    {
        private readonly ApplicationDbContext _db;

        public DrawParticipantRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(DrawParticipant drawParticipant)
        {
            _db.Update(drawParticipant);
        }

    }
}
