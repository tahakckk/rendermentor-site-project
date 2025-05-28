using System.ComponentModel.DataAnnotations;

namespace RenderMentor.Models
{
    public class Memberships
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Desc { get; set; }

        [MaxLength(60)]
        public string MetaTitle { get; set; }
        [MaxLength(160)]
        public string MetaDesc { get; set; }
    }
}
