using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RenderMentor.Models;

namespace RenderMentor.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Şema isimlerini belirt
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });
            
            // StudentCourse row will be deleted when it's related Student or Course deleted.
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student).WithMany(sc => sc.StudentCourses).HasForeignKey(c => c.StudentId);
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course).WithMany(sc => sc.StudentCourses).HasForeignKey(s => s.CourseId);

            modelBuilder.Entity<InstructorCategory>()
                .HasKey(ic => new { ic.InstructorId, ic.CourseCategoryId });

            modelBuilder.Entity<InstructorCategory>()
                .HasOne(ic => ic.Instructor).WithMany(ic => ic.InstructorCategories).HasForeignKey(c => c.InstructorId);
            modelBuilder.Entity<InstructorCategory>()
                .HasOne(ic => ic.CourseCategory).WithMany(ic => ic.InstructorCategories).HasForeignKey(i => i.CourseCategoryId);

            modelBuilder.Entity<Instructor>()
                .HasMany(p => p.Courses).WithOne(t => t.Instructor).OnDelete(DeleteBehavior.SetNull);
        }

        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<LectureQuestion> LectureQuestions { get; set; }
        public DbSet<LectureAnswer> LectureAnswers { get; set; }
        public DbSet<CourseGallery> CourseGalleries { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<HomeSlider> HomeSliders { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<Social> Socials { get; set; }
        public DbSet<Reference> References { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogAuthor> BlogAuthors { get; set; }
        public DbSet<BlogAudio> BlogAudios { get; set; }
        public DbSet<DrawParticipant> DrawParticipants { get; set; }
        public DbSet<Vendor> Vendors { get; set; }

        // Single Pages
        public DbSet<CourseList> CourseLists { get; set; }
        public DbSet<HomeContent> HomeContent { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<MentorPage> MentorPages { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Memberships> Memberships { get; set; }
        public DbSet<Faq> Faq { get; set; }

        // Cart, Order and Payments
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<TrialMember> TrialMembers { get; set; }

        // Media Galleries
        public DbSet<CourseCover> CourseCovers { get; set; }
        public DbSet<BlogThumb> BlogThumbs { get; set; }
        public DbSet<SlideBg> SlideBgs { get; set; }
    }
}
