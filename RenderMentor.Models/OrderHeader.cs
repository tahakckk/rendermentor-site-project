using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }        

        [Required]
        public DateTime OrderDate { get; set; }

        public double OrderTotal { get; set; }

        public string OrderStatus { get; set; }
        public string TransactionId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Ödeme, İade ve Teslimat Bilgileri formunu onaylamanız gereklidir.")]
        public bool TermsAndConditions { get; set; }

        public string OrderNumber { get; set; }

    }
}
