using RenderMentor.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class BlogDetailVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreateDate { get; set; }
        public BlogAuthor Author { get; set; }
        public string Image { get; set; }
        public string Desc { get; set; }
        public bool? Published { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDesc{ get; set; }

        public IEnumerable<BlogAudio> BlogAudios { get; set; }

        public List<BlogThumb> BlogThumbs { get; set; }
    }
}
