using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Student
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public virtual ApplicationUser ApplicationUser { get; set; }

    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual ICollection<LectureAnswer> LectureAnswers { get; set; } = new List<LectureAnswer>();

    public virtual ICollection<LectureQuestion> LectureQuestions { get; set; } = new List<LectureQuestion>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
