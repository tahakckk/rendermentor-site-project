using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public string Email { get; set; }

    public DateTime? CreatedDate { get; set; }
}
