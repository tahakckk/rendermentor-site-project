using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Membership
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public decimal? Price { get; set; }

    public string ImageUrl { get; set; }
}
