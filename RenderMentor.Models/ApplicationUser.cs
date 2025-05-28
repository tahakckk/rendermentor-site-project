using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(40)]
        public string Name { get; set; }
        public Genders Gender { get; set; }
        public enum Genders
        {
            [Display(Name = "Erkek")]
            Male = 1,
            [Display(Name = "Kadın")]
            Female = 2
        }

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public string Role { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
