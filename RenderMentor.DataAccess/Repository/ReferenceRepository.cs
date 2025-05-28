using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class ReferenceRepository : Repository<Reference>, IReferenceRepository
    {
        private readonly ApplicationDbContext _db;

        public ReferenceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Reference reference)
        {
            _db.Update(reference);
        }

    }
}
