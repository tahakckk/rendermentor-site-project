using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Lecture
{
    public int Id { get; set; }

    public int? CourseSectionId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string VideoUrl { get; set; }

    public int? Order { get; set; }

    public virtual CourseSection CourseSection { get; set; }

    public virtual ICollection<LectureQuestion> LectureQuestions { get; set; } = new List<LectureQuestion>();
}
