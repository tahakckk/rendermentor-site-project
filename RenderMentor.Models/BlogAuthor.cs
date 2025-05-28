using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models
{
    public class BlogAuthor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "İsim en fazla 50 karakter olmalıdır.")]
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int ListOrder { get; set; }
    }
}
