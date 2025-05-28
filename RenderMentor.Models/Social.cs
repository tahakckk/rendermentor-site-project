using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class Social
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Socials Name { get; set; }
        [Required]
        public string Link { get; set; }

        public int ListOrder { get; set; }

        public enum Socials
        {            
            Facebook = 1,            
            Instagram = 2,
            Twitter = 3,
            Linkedin = 4,
            Youtube = 5,
            Vimeo = 6
        }
    }
}
