using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class TrialMemberRepository : Repository<TrialMember>, ITrialMemberRepository
    {
        private readonly ApplicationDbContext _db;

        public TrialMemberRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(TrialMember trialMember)
        {
            _db.Update(trialMember);
        }
    }
}
