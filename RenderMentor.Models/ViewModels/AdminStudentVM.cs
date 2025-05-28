using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static RenderMentor.Models.ApplicationUser;

namespace RenderMentor.Models.ViewModels
{
    public class AdminStudentVM
    {

        public string Id { get; set; }

        [Required(ErrorMessage = "İsim alanı gereklidir.")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Adresi")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [Phone(ErrorMessage = "Lütfen geçerli bir telefon numarası giriniz.")]
        public string PhoneNumber { get; set; }

        public Genders Gender { get; set; }


        [StringLength(100, ErrorMessage = "Parola en az {2} en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "Parolanız en az bir büyük ve bir küçük harf ile en az bir rakam içermelidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "Parola ve tekrarı birbiriyle uyuşmuyor.")]
        public string ConfirmPassword { get; set; }

        public bool SubscribeActive { get; set; }
        public string SubscribePackageName { get; set; }
        public DateTime SubscribeStartDate { get; set; }
        public DateTime SubscribeExpireDate { get; set; }

        public int? CompanyId { get; set; }
        public string Role { get; set; }
        public IEnumerable<SelectListItem> CompanyList { get; set; }

        public ICollection<Course> AllCourses { get; set; }

        public List<StudentCourse> SelectedCourses { get; set; }

    }
}
