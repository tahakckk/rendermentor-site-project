using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderMentor.Models
{
    [Table("SlideBg", Schema = "dbo")]
    public class SlideBg
    {
        [Key]
        public int Id { get; set; }
        public string Image { get; set; }
        public string ImageSM { get; set; }
        public int? HomeSliderId { get; set; }

        [ForeignKey("HomeSliderId")]
        public virtual HomeSlider HomeSlider { get; set; }
    }
}
