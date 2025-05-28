using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class SlideBgRepository : Repository<SlideBg>, ISlideBgRepository
    {
        private readonly ApplicationDbContext _db;

        public SlideBgRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SlideBg slideBg)
        {
            var objFromDb = _db.SlideBgs.FirstOrDefault(s => s.Id == slideBg.Id);
            if (objFromDb != null)
            {
                objFromDb.Image = slideBg.Image;
                objFromDb.HomeSliderId = slideBg.HomeSliderId;
            }
        }
    }
}
