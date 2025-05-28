using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        void Update(Subscription subscription);
    }
}
