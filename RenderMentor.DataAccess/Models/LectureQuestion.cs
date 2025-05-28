using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class LectureQuestion
{
    public int Id { get; set; }

    public int? LectureId { get; set; }

    public int? StudentId { get; set; }

    public string Question { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Lecture Lecture { get; set; }

    public virtual ICollection<LectureAnswer> LectureAnswers { get; set; } = new List<LectureAnswer>();

    public virtual Student Student { get; set; }
}
