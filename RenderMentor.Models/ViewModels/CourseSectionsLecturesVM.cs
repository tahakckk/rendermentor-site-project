using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class CourseSectionsLecturesVM
    {
        public Course Course { get; set; }

        public IEnumerable<CourseSection> CourseSections { get; set; }

        public IEnumerable<Lecture> Lectures { get; set; }

        public int WatchingLectureId { get; set; }

        public int StudentId { get; set; }

        public Instructor Instructor { get; set; }

        public IEnumerable<InstructorCategory> InstructorCategories { get; set; }

        public bool StudentPackageActive { get; set; }

        public int[] StudentCourseIDs { get; set; }
    }
}
