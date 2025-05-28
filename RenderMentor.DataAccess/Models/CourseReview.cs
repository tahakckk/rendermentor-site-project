using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class CourseReview
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? StudentId { get; set; }

    public int? Rating { get; set; }

    public string Comment { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Course Course { get; set; }

    public virtual Student Student { get; set; }
}
