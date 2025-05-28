using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Models;

namespace RenderMentor.DataAccess.Data;

public partial class RenderMentorLocalContext : DbContext
{
    public RenderMentorLocalContext()
    {
    }

    public RenderMentorLocalContext(DbContextOptions<RenderMentorLocalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<About> Abouts { get; set; }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogAudio> BlogAudios { get; set; }

    public virtual DbSet<BlogAuthor> BlogAuthors { get; set; }

    public virtual DbSet<BlogThumb> BlogThumbs { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<CourseCover> CourseCovers { get; set; }

    public virtual DbSet<CourseGallery> CourseGalleries { get; set; }

    public virtual DbSet<CourseList> CourseLists { get; set; }

    public virtual DbSet<CourseReview> CourseReviews { get; set; }

    public virtual DbSet<CourseSection> CourseSections { get; set; }

    public virtual DbSet<DrawParticipant> DrawParticipants { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<HomeContent> HomeContents { get; set; }

    public virtual DbSet<HomeSlider> HomeSliders { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<Lecture> Lectures { get; set; }

    public virtual DbSet<LectureAnswer> LectureAnswers { get; set; }

    public virtual DbSet<LectureQuestion> LectureQuestions { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<MentorPage> MentorPages { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderHeader> OrderHeaders { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<Reference> References { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<SlideBg> SlideBgs { get; set; }

    public virtual DbSet<Social> Socials { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TrialMember> TrialMembers { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=RenderMentorLocal;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<About>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__About__3214EC07ED5A3723");

            entity.ToTable("About");
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC07E03FD236");

            entity.ToTable("ApplicationUser");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.Property(e => e.RoleId).IsRequired();

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Blog__3214EC077AA13176");

            entity.ToTable("Blog");

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Blog__AuthorId__0E240DFC");
        });

        modelBuilder.Entity<BlogAudio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlogAudi__3214EC07E1CAB9EB");

            entity.ToTable("BlogAudio");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogAudios)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK__BlogAudio__BlogI__11007AA7");
        });

        modelBuilder.Entity<BlogAuthor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlogAuth__3214EC078E6630FC");

            entity.ToTable("BlogAuthor");
        });

        modelBuilder.Entity<BlogThumb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlogThum__3214EC07C0E18578");

            entity.ToTable("BlogThumb");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogThumbs)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK__BlogThumb__BlogI__13DCE752");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Company__3214EC07F1A8DE1A");

            entity.ToTable("Company");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contact__3214EC075C10ED99");

            entity.ToTable("Contact");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contract__3214EC076D1A6A60");

            entity.ToTable("Contract");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC07BD844FBD");

            entity.ToTable("Course");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Course__Category__670A40DB");

            entity.HasOne(d => d.Instructor).WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstructorId)
                .HasConstraintName("FK__Course__Instruct__67FE6514");
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseCa__3214EC075344BA8D");

            entity.ToTable("CourseCategory");
        });

        modelBuilder.Entity<CourseCover>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseCo__3214EC0750D5B389");

            entity.ToTable("CourseCover");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseCovers)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__CourseCov__Cours__095F58DF");
        });

        modelBuilder.Entity<CourseGallery>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseGa__3214EC07A0D00F73");

            entity.ToTable("CourseGallery");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseGalleries)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__CourseGal__Cours__0682EC34");
        });

        modelBuilder.Entity<CourseList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseLi__3214EC07A5C05003");

            entity.ToTable("CourseList");
        });

        modelBuilder.Entity<CourseReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseRe__3214EC077FB89C5A");

            entity.ToTable("CourseReview");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__CourseRev__Cours__02B25B50");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__CourseRev__Stude__03A67F89");
        });

        modelBuilder.Entity<CourseSection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseSe__3214EC07BFCB2CA0");

            entity.ToTable("CourseSection");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseSections)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__CourseSec__Cours__6ADAD1BF");
        });

        modelBuilder.Entity<DrawParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DrawPart__3214EC075BA9BDBE");

            entity.ToTable("DrawParticipant");
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Faq__3214EC07C466A0CA");

            entity.ToTable("Faq");
        });

        modelBuilder.Entity<HomeContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HomeCont__3214EC07BBE879B2");

            entity.ToTable("HomeContent");

            entity.Property(e => e.MentorName).HasMaxLength(50);
            entity.Property(e => e.MetaDesc).HasMaxLength(160);
            entity.Property(e => e.MetaTitle).HasMaxLength(60);
            entity.Property(e => e.SubTitle).HasMaxLength(30);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<HomeSlider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HomeSlid__3214EC076999167A");

            entity.ToTable("HomeSlider");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Instruct__3214EC0739DF1048");

            entity.ToTable("Instructor");

            entity.HasMany(d => d.CourseCategories).WithMany(p => p.Instructors)
                .UsingEntity<Dictionary<string, object>>(
                    "InstructorCategory",
                    r => r.HasOne<CourseCategory>().WithMany()
                        .HasForeignKey("CourseCategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Instructo__Cours__7FD5EEA5"),
                    l => l.HasOne<Instructor>().WithMany()
                        .HasForeignKey("InstructorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Instructo__Instr__7EE1CA6C"),
                    j =>
                    {
                        j.HasKey("InstructorId", "CourseCategoryId").HasName("PK__Instruct__E9D77420128C6191");
                        j.ToTable("InstructorCategory");
                    });
        });

        modelBuilder.Entity<Lecture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lecture__3214EC07FBC5084E");

            entity.ToTable("Lecture");

            entity.HasOne(d => d.CourseSection).WithMany(p => p.Lectures)
                .HasForeignKey(d => d.CourseSectionId)
                .HasConstraintName("FK__Lecture__CourseS__6DB73E6A");
        });

        modelBuilder.Entity<LectureAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LectureA__3214EC077C84283B");

            entity.ToTable("LectureAnswer");

            entity.HasOne(d => d.LectureQuestion).WithMany(p => p.LectureAnswers)
                .HasForeignKey(d => d.LectureQuestionId)
                .HasConstraintName("FK__LectureAn__Lectu__7740A8A4");

            entity.HasOne(d => d.Student).WithMany(p => p.LectureAnswers)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__LectureAn__Stude__7834CCDD");
        });

        modelBuilder.Entity<LectureQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LectureQ__3214EC07DBD4AA8C");

            entity.ToTable("LectureQuestion");

            entity.HasOne(d => d.Lecture).WithMany(p => p.LectureQuestions)
                .HasForeignKey(d => d.LectureId)
                .HasConstraintName("FK__LectureQu__Lectu__737017C0");

            entity.HasOne(d => d.Student).WithMany(p => p.LectureQuestions)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__LectureQu__Stude__74643BF9");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Membersh__3214EC0715CA890D");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mentor__3214EC07981AC042");

            entity.ToTable("Mentor");
        });

        modelBuilder.Entity<MentorPage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MentorPa__3214EC077E68B664");

            entity.ToTable("MentorPage");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC0752F63496");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Course).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__OrderDeta__Cours__2F8501C7");

            entity.HasOne(d => d.OrderHeader).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderHeaderId)
                .HasConstraintName("FK__OrderDeta__Order__2E90DD8E");
        });

        modelBuilder.Entity<OrderHeader>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderHea__3214EC077DC4D16D");

            entity.ToTable("OrderHeader");

            entity.Property(e => e.ApplicationUserId).HasMaxLength(450);
            entity.Property(e => e.OrderTotal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.OrderHeaders)
                .HasForeignKey(d => d.ApplicationUserId)
                .HasConstraintName("FK__OrderHead__Appli__2BB470E3");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentT__3214EC07177C7A4B");

            entity.ToTable("PaymentTransaction");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.OrderHeader).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.OrderHeaderId)
                .HasConstraintName("FK__PaymentTr__Order__32616E72");
        });

        modelBuilder.Entity<Reference>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Referenc__3214EC0735083D3E");

            entity.ToTable("Reference");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shopping__3214EC07F626C528");

            entity.ToTable("ShoppingCart");

            entity.Property(e => e.ApplicationUserId).HasMaxLength(450);

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.ApplicationUserId)
                .HasConstraintName("FK__ShoppingC__Appli__3726238F");

            entity.HasOne(d => d.Course).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__ShoppingC__Cours__381A47C8");
        });

        modelBuilder.Entity<SlideBg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SlideBg__3214EC07AEC6A07E");

            entity.ToTable("SlideBg");

            entity.HasOne(d => d.HomeSlider).WithMany(p => p.SlideBgs)
                .HasForeignKey(d => d.HomeSliderId)
                .HasConstraintName("FK__SlideBg__HomeSli__3AF6B473");
        });

        modelBuilder.Entity<Social>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Social__3214EC07577C3CA5");

            entity.ToTable("Social");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Student__3214EC0729CED72F");

            entity.ToTable("Student");

            entity.Property(e => e.ApplicationUserId).HasMaxLength(450);

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.Students)
                .HasForeignKey(d => d.ApplicationUserId)
                .HasConstraintName("FK__Student__Applica__7093AB15");

            entity.HasMany(d => d.Courses).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentCo__Cours__7C055DC1"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentCo__Stude__7B113988"),
                    j =>
                    {
                        j.HasKey("StudentId", "CourseId").HasName("PK__StudentC__5E57FC83F2120555");
                        j.ToTable("StudentCourse");
                    });
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC074D007A8F");

            entity.ToTable("Subscription");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Team__3214EC07857EFC32");

            entity.ToTable("Team");
        });

        modelBuilder.Entity<TrialMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TrialMem__3214EC07EFB53EEF");

            entity.ToTable("TrialMember");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vendor__3214EC07BB86F0CB");

            entity.ToTable("Vendor");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
