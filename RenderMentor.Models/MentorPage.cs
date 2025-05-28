using System.ComponentModel.DataAnnotations;

namespace RenderMentor.Models
{
    public class MentorPage
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string PageTitle { get; set; }
        public string PageDesc { get; set; }
        public string PageBg { get; set; }

        [MaxLength(60)]
        public string MetaTitle { get; set; }
        [MaxLength(160)]
        public string MetaDesc { get; set; }
    }
}
