using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    [Table("HomeContent", Schema = "dbo")]
    public class HomeContent
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        public string SubTitle { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Desc { get; set; }

        [MaxLength(50)]
        public string MentorName { get; set; }
        public string MentorTitle { get; set; }
        public string MentorImage { get; set; }

        [MaxLength(60)]
        public string MetaTitle { get; set; }
        [MaxLength(160)]
        public string MetaDesc { get; set; }

        public bool DrawOnline { get; set; }
        public string DrawTitle { get; set; }
        public string DrawDesc { get; set; }
        public string DrawSmallDesc { get; set; }
        public bool TrialOnline { get; set; }
    }
}
