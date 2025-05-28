using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RenderMentor.ViewComponents
{
    public class VendorViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public VendorViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            var vendors = await _db.Vendors.OrderBy(i => i.ListOrder).ToListAsync();
            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, vendors);
            }
            return View(vendors);
        }
    }
}
