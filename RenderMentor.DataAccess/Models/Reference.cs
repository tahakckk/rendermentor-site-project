using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Reference
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }
}
