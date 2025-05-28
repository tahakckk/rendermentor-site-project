using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class CourseVM
    {
        public Course Course { get; set; }
        public IEnumerable<SelectListItem> CourseCategoryList { get; set; }
        public IEnumerable<SelectListItem> CourseInstructorList { get; set; }
        public Lecture Lecture { get; set; }

        public IEnumerable<SelectListItem> CourseSectionList { get; set; }

        public string CategoryURL { get; set; }
    }
}
