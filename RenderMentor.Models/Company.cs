using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models
{
    public class Company
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        [Display(Name = "Firma Adı")]
        public string Name { get; set; }

        [MaxLength(120)]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string District { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
