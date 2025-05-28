using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class BlogAudio
{
    public int Id { get; set; }

    public int? BlogId { get; set; }

    public string AudioUrl { get; set; }

    public virtual Blog Blog { get; set; }
}
