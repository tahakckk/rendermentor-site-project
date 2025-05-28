using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class FaqRepository : Repository<Faq>, IFaqRepository
    {
        private readonly ApplicationDbContext _db;

        public FaqRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Faq faq)
        {
            var objFromDb = _db.Faq.FirstOrDefault(s => s.Id == faq.Id);
            if (objFromDb != null)
            {
                objFromDb.Question = faq.Question;
                objFromDb.Answer = faq.Answer;
            }
        }

    }
}
