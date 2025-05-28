using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IReferenceRepository : IRepository<Reference>
    {
        void Update(Reference reference);
    }
}
