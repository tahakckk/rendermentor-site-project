using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IMentorRepository : IRepository<Mentor>
    {
        void Update(Mentor mentor);
    }
}
