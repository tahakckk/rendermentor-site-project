using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class CourseGallery
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        public int ListOrder { get; set; }

        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
