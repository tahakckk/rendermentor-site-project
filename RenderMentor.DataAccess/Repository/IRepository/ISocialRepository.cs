using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ISocialRepository : IRepository<Social>
    {
        void Update(Social social);
    }
}
