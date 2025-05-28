using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RenderMentor.ViewComponents
{
    public class ReferenceViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public ReferenceViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            var reference = await _db.References.OrderBy(i => i.ListOrder).ToListAsync();
            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, reference);
            }
            return View(reference);
        }
    }
}
