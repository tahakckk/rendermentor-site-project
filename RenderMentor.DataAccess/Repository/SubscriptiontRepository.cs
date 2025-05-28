using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class SubscriptiontRepository : Repository<Subscription>, ISubscriptionRepository
    {
        private readonly ApplicationDbContext _db;

        public SubscriptiontRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Subscription subscription)
        {
            _db.Update(subscription);
        }

    }
}
