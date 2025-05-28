using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Contract
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Content { get; set; }
}
