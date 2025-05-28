using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class LectureQuestionAdminVM
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public DateTime DateTime { get; set; }
        public string Question { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int CourseSectionId { get; set; }
        public string CourseSectionName { get; set; }
        public string LectureName { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public bool Answered { get; set; }
        public int AnswerCount { get; set; }

        public IEnumerable<LectureAnswerVM> LectureAnswers { get; set; }
    }
}
