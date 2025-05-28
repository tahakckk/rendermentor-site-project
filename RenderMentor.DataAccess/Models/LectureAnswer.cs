using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class LectureAnswer
{
    public int Id { get; set; }

    public int? LectureQuestionId { get; set; }

    public int? StudentId { get; set; }

    public string Answer { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual LectureQuestion LectureQuestion { get; set; }

    public virtual Student Student { get; set; }
}
