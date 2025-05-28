using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class LectureQuestionVM
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Question { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int CourseId { get; set; }

        public IEnumerable<LectureAnswerVM> LectureAnswers { get; set; }

    }
}
