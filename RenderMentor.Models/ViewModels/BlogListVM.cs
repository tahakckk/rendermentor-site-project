using RenderMentor.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class BlogListVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreateDate { get; set; }
        public BlogAuthor Author { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailSM { get; set; }
        public string Desc { get; set; }
        public string SEOUrl { get; set; }
        public string ShortDesc
        {
            get
            {
                string processed = string.Empty;
                if (Desc != null)
                {
                    processed = StringProcessing.ParseOutHTML(Desc);
                    if (processed.Length > 140)
                    {
                        processed = processed.Substring(0, 140) + "...";
                    }
                }
                return (processed);
            }
        }

        public List<BlogThumb> BlogThumbs { get; set; }
    }
}
