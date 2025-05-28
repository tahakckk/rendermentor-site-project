using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using static RenderMentor.Models.ApplicationUser;

namespace RenderMentor.Models.ViewModels
{
    public class AdminInstructorVM
    {

        public int Id { get; set; }

        public string UserId { get; set; }

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

        public string Desc { get; set; }

        public string AvatarImage { get; set; }
        public string Image { get; set; }

        [StringLength(100, ErrorMessage = "Parola en az {2} en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessage = "Parolanız en az bir büyük ve bir küçük harf ile en az bir rakam içermelidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Parola Tekrarı")]
        [Compare("Password", ErrorMessage = "Parola ve tekrarı birbiriyle uyuşmuyor.")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }

        public ICollection<CourseCategory> AllCategories { get; set; }

        public ICollection<int> SelectedCategories { get; set; }

        public bool IsAdmin { get; set; }

    }
}
