using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class BlogThumb
{
    public int Id { get; set; }

    public int? BlogId { get; set; }

    public string ImageUrl { get; set; }

    public virtual Blog Blog { get; set; }
}
