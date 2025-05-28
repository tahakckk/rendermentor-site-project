using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IVendorRepository : IRepository<Vendor>
    {
        void Update(Vendor vendor);
    }
}
