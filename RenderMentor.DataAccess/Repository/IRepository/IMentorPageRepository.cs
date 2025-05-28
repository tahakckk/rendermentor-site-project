using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IMentorPageRepository : IRepository<MentorPage>
    {
        void Update(MentorPage mentorPage);
    }
}
