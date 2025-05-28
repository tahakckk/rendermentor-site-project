using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class About
{
    public int Id { get; set; }

    public string PageTitle { get; set; }

    public string PageDesc { get; set; }

    public string PageImage { get; set; }

    public string PageBg { get; set; }

    public string MetaTitle { get; set; }

    public string MetaDesc { get; set; }
}
