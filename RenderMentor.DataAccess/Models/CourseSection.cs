using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class CourseSection
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public string Title { get; set; }

    public int? Order { get; set; }

    public virtual Course Course { get; set; }

    public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}
