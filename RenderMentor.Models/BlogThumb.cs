using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class BlogThumb
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string ImageSM { get; set; }
        [Required]
        public string ImageXSM { get; set; }
        public int? BlogId { get; set; }
    }
}
