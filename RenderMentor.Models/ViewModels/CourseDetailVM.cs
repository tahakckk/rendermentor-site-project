using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class CourseDetailVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public bool Published { get; set; }
        public bool OnSale { get; set; }
        public string Description { get; set; }
        public string Intro { get; set; }

        public string MetaTitle { get; set; }
        public string MetaDesc { get; set; }
        public string CoverImage { get; set; }

        public TimeSpan TotalDuration { get; set; }

        public List<CourseSection> CourseSections { get; set; }
        public List<CourseGallery> CourseGallery { get; set; }

        public int TotalLectures { get; set; }

        public CourseCategory CourseCategory { get; set; }

        public Instructor Instructor { get; set; }

        public IEnumerable<InstructorCategory> InstructorCategories { get; set; }

        public string CourseListBg { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        // Check if the registered student has the course
        public bool StudentPackageActive { get; set; }
        public int[] StudentCourseIDs { get; set; }
        public bool StudentHasReview { get; set; }

        public double? Rating { get; set; }
        public int? RatingCount { get; set; }

        public IEnumerable<CourseReview> CourseReviews { get; set; }
    }
}
