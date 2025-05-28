using Microsoft.AspNetCore.Mvc.Rendering;
using RenderMentor.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class StudentCourseVM
    {
        public Course Course { get; set; }
        public bool StudentHasReview { get; set; }
    }
}
