using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Desc { get; set; }
        public int ListOrder { get; set; }

        public string SEOUrl { get; set; }
    }
}
