using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class CourseList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string PageBg { get; set; }

        
        [MaxLength(60)]
        public string MetaTitle { get; set; }
        [MaxLength(160)]
        public string MetaDesc { get; set; }
        public string SEOUrl { get; set; }
    }
}
