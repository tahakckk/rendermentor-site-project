using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderMentor.Models
{
    [Table("About", Schema = "dbo")]
    public class About
    {
        [Key]
        public int Id { get; set; }
        public string PageTitle { get; set; }
        public string PageDesc { get; set; }
        public string PageImage { get; set; }
        public string PageBg { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDesc { get; set; }
    }
}
