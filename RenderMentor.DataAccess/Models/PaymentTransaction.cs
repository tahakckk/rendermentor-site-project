using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class PaymentTransaction
{
    public int Id { get; set; }

    public int? OrderHeaderId { get; set; }

    public string TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public virtual OrderHeader OrderHeader { get; set; }
}
