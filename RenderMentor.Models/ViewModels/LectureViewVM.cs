using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class LectureViewVM
    {
        public Lecture Lecture { get; set; }

        public IEnumerable<LectureQuestionVM> LectureQuestions { get; set; }

    }
}
