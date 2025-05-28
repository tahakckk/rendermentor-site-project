using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(1, 10000, ErrorMessage = "Fiyat alanı için {1} ile {2} arası bir değer girmeniz gereklidir.")]
        public double Price { get; set; }
        [Range(0, 1000, ErrorMessage = "Üyelik süresi için {1} ile {2} arası bir değer girmeniz gereklidir.")]
        public double ExpirationDays { get; set; }
        public bool IsActive { get; set; }        

        [MaxLength(60)]
        public string MetaTitle { get; set; }
        [MaxLength(160)]
        public string MetaDesc { get; set; }
    }
}
