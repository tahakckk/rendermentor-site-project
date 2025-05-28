using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Subject { get; set; }

    public string Message { get; set; }
}
