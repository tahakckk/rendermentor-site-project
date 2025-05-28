using System.ComponentModel.DataAnnotations;
using static RenderMentor.Models.ApplicationUser;

namespace RenderMentor.Models.ViewModels
{
    public class StudentVM
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "İsim alanı gereklidir.")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "Lütfen geçerli bir telefon numarası giriniz.")]
        public string PhoneNumber { get; set; }

        public Genders Gender { get; set; }

        public string StatusMessage { get; set; }

    }
}
