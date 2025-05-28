using System;
using System.Collections.Generic;

namespace RenderMentor.DataAccess.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public decimal? Price { get; set; }

    public string ImageUrl { get; set; }

    public int? CategoryId { get; set; }

    public int? InstructorId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string MetaTitle { get; set; }

    public string MetaDescription { get; set; }

    public virtual CourseCategory Category { get; set; }

    public virtual ICollection<CourseCover> CourseCovers { get; set; } = new List<CourseCover>();

    public virtual ICollection<CourseGallery> CourseGalleries { get; set; } = new List<CourseGallery>();

    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual ICollection<CourseSection> CourseSections { get; set; } = new List<CourseSection>();

    public virtual Instructor Instructor { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
