using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class Lecture
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Video { get; set; }
        public bool IsPreview { get; set; }
        public string Description { get; set; }
        public string MainDescription { get; set; }
        public TimeSpan Duration { get; set; }
        public int ListOrder { get; set; }

        [Required]
        public int CourseSectionId { get; set; }
        [ForeignKey("CourseSectionId")]
        public CourseSection CourseSection { get; set; }

        public int CourseId { get; set; }
    }
}
