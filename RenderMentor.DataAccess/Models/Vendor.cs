using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Vendor
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }
}
