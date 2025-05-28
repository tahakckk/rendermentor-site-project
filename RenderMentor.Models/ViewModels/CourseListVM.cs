using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class CourseListVM
    {        
        public IEnumerable<Course> Courses { get; set; }

        public CourseList CourseList { get; set; }

        public CourseCategory CourseCategory { get; set; }

        public string courseCategory { get; set; }
    }
}
