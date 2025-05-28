using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class HomeSliderRepository : Repository<HomeSlider>, IHomeSliderRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeSliderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(HomeSlider slider)
        {
            var objFromDb = _db.HomeSliders.FirstOrDefault(s => s.Id == slider.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = slider.Title;
                objFromDb.Desc = slider.Desc;
                if (slider.SlideBg != null)
                {
                    objFromDb.SlideBg = slider.SlideBg;
                }
                objFromDb.ButtonLink = slider.ButtonLink;
                objFromDb.ButtonText = slider.ButtonText;
            }
        }

    }
}
