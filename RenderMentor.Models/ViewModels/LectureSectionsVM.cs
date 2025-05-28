using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class LectureSectionsVM
    {
        public Lecture Lecture { get; set; }

        public IEnumerable<CourseSection> CourseSections { get; set; }

    }
}
