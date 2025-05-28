using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class ApplicationUser
{
    public string Id { get; set; }

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string Email { get; set; }

    public string NormalizedEmail { get; set; }

    public bool? EmailConfirmed { get; set; }

    public string PasswordHash { get; set; }

    public string SecurityStamp { get; set; }

    public string ConcurrencyStamp { get; set; }

    public string PhoneNumber { get; set; }

    public bool? PhoneNumberConfirmed { get; set; }

    public bool? TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool? LockoutEnabled { get; set; }

    public int? AccessFailedCount { get; set; }

    public string Name { get; set; }

    public string StreetAddress { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string PostalCode { get; set; }

    public string PhoneNumber2 { get; set; }

    public virtual ICollection<OrderHeader> OrderHeaders { get; set; } = new List<OrderHeader>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
