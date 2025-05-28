using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RenderMentor.ViewComponents
{
    public class MentorsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public MentorsViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            var mentors = await _db.Mentors.OrderBy(i => i.ListOrder).ToListAsync();
            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, mentors);
            }
            return View(mentors);
        }
    }
}
