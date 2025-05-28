using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class OrderHeader
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? OrderTotal { get; set; }

    public string OrderStatus { get; set; }

    public string PaymentStatus { get; set; }

    public string TransactionId { get; set; }

    public virtual ApplicationUser ApplicationUser { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
