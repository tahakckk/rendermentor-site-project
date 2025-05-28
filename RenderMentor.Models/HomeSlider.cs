using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    [Table("HomeSlider", Schema = "dbo")]
    public class HomeSlider
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        [Required]
        public string SlideBg { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public int ListOrder { get; set; }

        public virtual ICollection<SlideBg> SlideBgs { get; set; }
    }
}
