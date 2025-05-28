using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IMembershipsRepository : IRepository<Memberships>
    {
        void Update(Memberships memberships);
    }
}
