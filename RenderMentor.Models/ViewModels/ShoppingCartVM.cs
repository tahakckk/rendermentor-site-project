using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }

        public OrderHeader OrderHeader { get; set; }

        [Required(ErrorMessage = "Lütfen kredi kartında yer alan isim ve soyadı yazınız.")]
        public string CardHolderName { get; set; }

        [Required(ErrorMessage = "Lütfen 16 haneli kart numaranızı yazınız.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Kart numaranız 16 haneli olmalıdır.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Lütfen kartın son kullanma tarihinin ay kısmını 2 haneli olarak yazınız.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Son kullanma tarihi ay kısmı 2 haneli olmalıdır.")]
        public string ExpireMonth { get; set; }

        [Required(ErrorMessage = "Lütfen kartın son kullanma tarihinin yıl kısmını 2 haneli olarak yazınız.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Son kullanma tarihi yıl kısmı 2 haneli olmalıdır.")]
        public string ExpireYear { get; set; }

        [Required(ErrorMessage = "Lütfen kartın cvv numarasını yazınız.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV numarası 3 haneli olmalıdır.")]
        public string CvvCode { get; set; }

        public string OrderNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
