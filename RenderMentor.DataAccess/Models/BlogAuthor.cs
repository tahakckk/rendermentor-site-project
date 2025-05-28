using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class BlogAuthor
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
