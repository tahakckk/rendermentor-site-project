using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IFaqRepository : IRepository<Faq>
    {
        void Update(Faq faq);
    }
}
