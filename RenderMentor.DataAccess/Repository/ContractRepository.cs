using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        private readonly ApplicationDbContext _db;

        public ContractRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Contract contract)
        {
            var objFromDb = _db.Contracts.FirstOrDefault(s => s.Id == contract.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = contract.Name;
                objFromDb.Desc = contract.Desc;
                objFromDb.SEOUrl = contract.SEOUrl;
            }
        }

    }
}
