using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class ShoppingCart
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }

    public int? CourseId { get; set; }

    public int? Count { get; set; }

    public virtual ApplicationUser ApplicationUser { get; set; }

    public virtual Course Course { get; set; }
}
