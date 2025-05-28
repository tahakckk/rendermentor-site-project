using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int? OrderHeaderId { get; set; }

    public int? CourseId { get; set; }

    public decimal? Price { get; set; }

    public virtual Course Course { get; set; }

    public virtual OrderHeader OrderHeader { get; set; }
}
