using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
    {
        void Update(PaymentTransaction payment);
    }
}