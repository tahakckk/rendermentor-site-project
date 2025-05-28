using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class SlideBg
{
    public int Id { get; set; }

    public int? HomeSliderId { get; set; }

    public string ImageUrl { get; set; }

    public virtual HomeSlider HomeSlider { get; set; }
}
