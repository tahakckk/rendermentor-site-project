using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class HomeContent
{
    public int Id { get; set; }

    public string SubTitle { get; set; }

    public string Title { get; set; }

    public string ShortDesc { get; set; }

    public string Desc { get; set; }

    public string MentorName { get; set; }

    public string MentorTitle { get; set; }

    public string MentorImage { get; set; }

    public string MetaTitle { get; set; }

    public string MetaDesc { get; set; }

    public bool? DrawOnline { get; set; }

    public string DrawTitle { get; set; }

    public string DrawDesc { get; set; }

    public string DrawSmallDesc { get; set; }

    public bool? TrialOnline { get; set; }
}
