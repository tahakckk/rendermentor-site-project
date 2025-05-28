using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RenderMentor.Models.ViewModels
{
    public class BlogVM
    {
        public Blog Blog { get; set; }
        public IEnumerable<SelectListItem> BlogAuthorList { get; set; }
    }
}
