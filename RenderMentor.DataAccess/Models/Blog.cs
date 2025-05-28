using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Blog
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? AuthorId { get; set; }

    public int? CategoryId { get; set; }

    public string ImageUrl { get; set; }

    public string MetaTitle { get; set; }

    public string MetaDescription { get; set; }

    public virtual BlogAuthor Author { get; set; }

    public virtual ICollection<BlogAudio> BlogAudios { get; set; } = new List<BlogAudio>();

    public virtual ICollection<BlogThumb> BlogThumbs { get; set; } = new List<BlogThumb>();
}
