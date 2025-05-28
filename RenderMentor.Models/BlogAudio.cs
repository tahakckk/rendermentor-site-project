using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class BlogAudio
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Audio { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(450)]
        public string Desc { get; set; }
        public int ListOrder { get; set; }

        [Required]
        public int BlogId { get; set; }
        [ForeignKey("BlogId")]
        public Blog Blog { get; set; }
    }
}
