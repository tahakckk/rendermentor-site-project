using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RenderMentor.ViewComponents
{
    public class CourseCategoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public CourseCategoryViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            var courseCategories = await _db.CourseCategories.OrderBy(i => i.ListOrder).ToListAsync();
            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, courseCategories);
            }
            return View(courseCategories);
        }
    }
}
