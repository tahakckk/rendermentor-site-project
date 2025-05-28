using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string AvatarImage { get; set; }

        public int ListOrder { get; set; }

    }
}
