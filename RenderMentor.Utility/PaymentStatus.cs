using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Utility
{
    public enum PaymentStatus
    {
        [Display(Name = "Beklemede")]
        Pending = 10,

        [Display(Name = "Ödendi")]
        Paid = 20,

        [Display(Name = "Hatalı")]
        Failed = 30,

        [Display(Name = "Gönderildi")]
        Shipped = 40
    }
}
