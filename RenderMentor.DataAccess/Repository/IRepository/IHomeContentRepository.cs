using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IHomeContentRepository : IRepository<HomeContent>
    {
        void Update(HomeContent homeContent);
    }
}
