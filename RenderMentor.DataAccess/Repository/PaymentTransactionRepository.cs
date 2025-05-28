using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
    {
        private readonly ApplicationDbContext _db;

        public PaymentTransactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(PaymentTransaction payment)
        {
            _db.Update(payment);
        }

    }
}
