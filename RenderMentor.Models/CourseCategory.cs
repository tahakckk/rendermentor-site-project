using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models
{
    public class CourseCategory
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        [Display(Name="Kategori Adı")]
        public string Name { get; set; }
        [MaxLength(160)]
        public string ShortDesc { get; set; }
        public string CoverImage { get; set; }
        public string PageTitle { get; set; }
        public string PageDesc { get; set; }
        public string PageBg { get; set; }
        [MaxLength(60)]
        public string MetaTitle { get; set; }
        [MaxLength(160)]
        public string MetaDesc { get; set; }
        public int ListOrder { get; set; }

        public string SEOUrl { get; set; }

        public IList<InstructorCategory> InstructorCategories { get; set; } = new List<InstructorCategory>();
    }
}
