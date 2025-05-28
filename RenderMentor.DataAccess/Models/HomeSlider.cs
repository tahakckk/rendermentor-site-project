using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class HomeSlider
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string ButtonLink { get; set; }

    public string ButtonText { get; set; }

    public string Desc { get; set; }

    public int? ListOrder { get; set; }

    public string SlideBg { get; set; }

    public virtual ICollection<SlideBg> SlideBgs { get; set; } = new List<SlideBg>();
}
