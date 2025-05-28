using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(1, 10000, ErrorMessage = "Fiyat alanı için {1} ile {2} arası bir değer girmeniz gereklidir.")]
        public double Price { get; set; }
        public bool Published { get; set; }
        public bool OnSale { get; set; }
        public bool IsTrial { get; set; }
        public string CoverImage { get; set; }

        // Intro video of the lecture
        public string Intro { get; set; }
        [MaxLength(60)]
        public string MetaTitle { get; set; }
        [MaxLength(160)]
        public string MetaDesc { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public int ListOrder { get; set; }

        public string SEOUrl { get; set; }

        public List<CourseSection> CourseSections { get; set; }
        public List<CourseGallery> CourseGallery { get; set; }
        public List<CourseCover> CourseCovers { get; set; }

        public IList<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        public int TotalLectures { get; set; }

        [Required]
        public int CourseCategoryId { get; set; }
        [ForeignKey("CourseCategoryId")]
        public CourseCategory CourseCategory { get; set; }

        public int? InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public Instructor Instructor { get; set; }

        public ICollection<CourseReview> Reviews { get; set; }
    }
}
