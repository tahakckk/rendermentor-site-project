using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class CourseSection
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public int ListOrder { get; set; }

        public List<Lecture> Lectures { get; set; }

        public int TotalLectures { get; set; }

        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
