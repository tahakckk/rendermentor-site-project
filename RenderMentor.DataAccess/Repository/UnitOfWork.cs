using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CourseCategory = new CourseCategoryRepository(_db);
            Course = new CourseRepository(_db);
            CourseSection = new CourseSectionRepository(_db);
            CourseGallery = new CourseGalleryRepository(_db);
            Lecture = new LectureRepository(_db);
            CourseReview = new CourseReviewRepository(_db);
            LectureQuestion = new LectureQuestionRepository(_db);
            LectureAnswer = new LectureAnswerRepository(_db);
            Subscription = new SubscriptiontRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            Instructor = new InstructorRepository(_db);
            Company = new CompanyRepository(_db);
            Student = new StudentRepository(_db);
            StudentCourse = new StudentCourseRepository(_db);
            InstructorCategory = new InstructorCategoryRepository(_db);
            CourseList = new CourseListRepository(_db);
            HomeContent = new HomeContentRepository(_db);
            About = new AboutRepository(_db);
            MentorPage = new MentorPageRepository(_db);
            Contact = new ContactRepository(_db);
            Memberships = new MembershipsRepository(_db);
            Faq = new FaqRepository(_db);
            HomeSlider = new HomeSliderRepository(_db);
            Team = new TeamRepository(_db);
            Mentor = new MentorRepository(_db);
            Social = new SocialRepository(_db);
            Reference = new ReferenceRepository(_db);
            Contract = new ContractRepository(_db);
            Blog = new BlogRepository(_db);
            BlogAuthor = new BlogAuthorRepository(_db);
            BlogAudio = new BlogAudioRepository(_db);
            DrawParticipant = new DrawParticipantRepository(_db);
            Vendor = new VendorRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            PaymentTransaction = new PaymentTransactionRepository(_db);
            TrialMember = new TrialMemberRepository(_db);
            CourseCover = new CourseCoverRepository(_db);
            BlogThumb = new BlogThumbRepository(_db);
            SlideBg = new SlideBgRepository(_db);
            SP_Call = new SP_Call(_db);
        }

        public ICourseCategoryRepository CourseCategory { get; private set; }
        public ICourseRepository Course { get; private set; }
        public ICourseSectionRepository CourseSection { get; private set; }
        public ICourseGalleryRepository CourseGallery { get; private set; }
        public ILectureRepository Lecture { get; private set; }
        public ICourseReviewRepository CourseReview { get; private set; }
        public ILectureQuestionRepository LectureQuestion { get; private set; }
        public ILectureAnswerRepository LectureAnswer { get; private set; }
        public ISubscriptionRepository Subscription { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IInstructorRepository Instructor { get; }
        public ICompanyRepository Company { get; private set; }
        public IStudentRepository Student { get; private set; }
        public IStudentCourseRepository StudentCourse { get; private set; }
        public IInstructorCategoryRepository InstructorCategory { get; private set; }
        public ICourseListRepository CourseList { get; private set; }
        public IHomeContentRepository HomeContent { get; private set; }
        public IAboutRepository About { get; private set; }
        public IMentorPageRepository MentorPage { get; private set; }
        public IContactRepository Contact { get; private set; }
        public IMembershipsRepository Memberships { get; private set; }
        public IFaqRepository Faq { get; private set; }
        public IHomeSliderRepository HomeSlider { get; private set; }
        public ITeamRepository Team { get; private set; }
        public IMentorRepository Mentor { get; private set; }
        public ISocialRepository Social { get; private set; }
        public IReferenceRepository Reference { get; private set; }
        public IContractRepository Contract { get; private set; }
        public IBlogRepository Blog { get; private set; }
        public IBlogAuthorRepository BlogAuthor { get; private set; }
        public IBlogAudioRepository BlogAudio { get; private set; }
        public IDrawParticipantRepository DrawParticipant { get; private set; }
        public IVendorRepository Vendor { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IPaymentTransactionRepository PaymentTransaction { get; private set; }
        public ITrialMemberRepository TrialMember { get; private set; }
        public ICourseCoverRepository CourseCover { get; private set; }
        public IBlogThumbRepository BlogThumb { get; private set; }
        public ISlideBgRepository SlideBg { get; private set; }
        public ISP_Call SP_Call { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
