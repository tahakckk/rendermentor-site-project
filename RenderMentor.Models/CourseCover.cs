using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class CourseCover
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        public string ImageSM { get; set; }
        public int? CourseId { get; set; }
    }
}
