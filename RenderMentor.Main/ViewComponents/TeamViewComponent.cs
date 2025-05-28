using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RenderMentor.ViewComponents
{
    public class TeamViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public TeamViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            var team = await _db.Teams.OrderBy(i => i.ListOrder).ToListAsync();
            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, team);
            }
            return View(team);
        }
    }
}
