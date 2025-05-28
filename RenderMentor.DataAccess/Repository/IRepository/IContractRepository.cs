using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IContractRepository : IRepository<Contract>
    {
        void Update(Contract contract);
    }
}
