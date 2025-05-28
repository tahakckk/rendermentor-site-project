using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RenderMentor.ViewComponents
{
    public class ContactViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public ContactViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
        {
            var contact = await _db.Contact.FirstOrDefaultAsync();
            if (contact == null)
            {
                contact = new Contact();
            }
            
            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, contact);
            }
            return View(contact);
        }
    }
}
