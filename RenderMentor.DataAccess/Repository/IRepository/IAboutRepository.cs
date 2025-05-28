using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IAboutRepository : IRepository<About>
    {
        void Update(About about);
    }
}
