using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public int PackageId { get; set; }
        [ForeignKey("PackageId")]
        public Subscription Subscription { get; set; }

        [NotMapped]
        public double Price { get; set; }
    }
}
