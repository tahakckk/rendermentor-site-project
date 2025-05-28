using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class VendorRepository : Repository<Vendor>, IVendorRepository
    {
        private readonly ApplicationDbContext _db;

        public VendorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Vendor vendor)
        {
            var objFromDb = _db.Vendors.FirstOrDefault(s => s.Id == vendor.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = vendor.Name;
                if (vendor.Image != null)
                {
                    objFromDb.Image = vendor.Image;
                }
                if (vendor.Thumbnail != null)
                {
                    objFromDb.Thumbnail = vendor.Thumbnail;
                }
                objFromDb.ShortDesc = vendor.ShortDesc;
                objFromDb.Desc = vendor.Desc;
                objFromDb.MetaTitle = vendor.MetaTitle;
                objFromDb.MetaDesc = vendor.MetaDesc;
                objFromDb.SEOUrl = vendor.SEOUrl;
            }
        }

    }
}
