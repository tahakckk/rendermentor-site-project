using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RenderMentor.Models.ViewModels
{
    public class StudentEmailVM
    {
        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Yeni Email")]
        public string NewEmail { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

    }
}
