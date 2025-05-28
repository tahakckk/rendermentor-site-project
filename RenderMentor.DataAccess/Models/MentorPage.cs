using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class MentorPage
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string MetaTitle { get; set; }

    public string MetaDescription { get; set; }
}
