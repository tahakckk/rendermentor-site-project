using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IDrawParticipantRepository : IRepository<DrawParticipant>
    {
        void Update(DrawParticipant drawParticipant);
    }
}
