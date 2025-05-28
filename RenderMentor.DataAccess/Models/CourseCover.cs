using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class CourseCover
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public string ImageUrl { get; set; }

    public virtual Course Course { get; set; }
}
