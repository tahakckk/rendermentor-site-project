using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ITeamRepository : IRepository<Team>
    {
        void Update(Team team);
    }
}
