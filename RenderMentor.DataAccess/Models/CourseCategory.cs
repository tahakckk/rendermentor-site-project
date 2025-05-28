using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class CourseCategory
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
}
