using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class LectureAnswerVM
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Answer { get; set; }
        public string Name { get; set; }
        public bool IsInstructor { get; set; }
    }
}
