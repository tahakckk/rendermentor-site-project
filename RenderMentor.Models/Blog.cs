using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Blog Başlığı")]
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }        
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailSM { get; set; }
        public string Desc { get; set; }

        public bool? Published { get; set; }

        public int ListOrder { get; set; }
        public string SEOUrl { get; set; }

        [MaxLength(60)]
        public string MetaTitle { get; set; }
        [MaxLength(160)]
        public string MetaDesc { get; set; }

        public int? AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public BlogAuthor Author { get; set; }

        public List<BlogThumb> BlogThumbs { get; set; }

    }
}
