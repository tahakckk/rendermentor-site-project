using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseCategoryRepository CourseCategory { get; }
        ICourseRepository Course { get; }
        ICourseSectionRepository CourseSection { get; }
        ICourseGalleryRepository CourseGallery { get; }
        ILectureRepository Lecture { get; }
        ICourseReviewRepository CourseReview { get; }
        ILectureQuestionRepository LectureQuestion { get; }
        ILectureAnswerRepository LectureAnswer { get; }
        ISubscriptionRepository Subscription { get; }

        IApplicationUserRepository ApplicationUser { get; }
        IInstructorRepository Instructor { get; }
        ICompanyRepository Company { get; }
        IStudentRepository Student { get; }
        IStudentCourseRepository StudentCourse { get; }
        IInstructorCategoryRepository InstructorCategory { get; }
        ICourseListRepository CourseList { get; }
        IHomeContentRepository HomeContent { get; }
        IAboutRepository About { get; }
        IMentorPageRepository MentorPage { get; }
        IContactRepository Contact { get; }
        IMembershipsRepository Memberships { get; }
        IFaqRepository Faq { get; }
        IHomeSliderRepository HomeSlider { get; }
        ITeamRepository Team { get; }
        IMentorRepository Mentor { get; }
        ISocialRepository Social { get; }
        IReferenceRepository Reference { get; }
        IContractRepository Contract { get; }
        IBlogRepository Blog { get; }
        IBlogAuthorRepository BlogAuthor { get; }
        IBlogAudioRepository BlogAudio { get; }
        IDrawParticipantRepository DrawParticipant { get; }
        IVendorRepository Vendor { get; }

        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
        IPaymentTransactionRepository PaymentTransaction { get; }
        ITrialMemberRepository TrialMember { get; }

        ICourseCoverRepository CourseCover { get; }
        IBlogThumbRepository BlogThumb { get; }
        ISlideBgRepository SlideBg { get; }
        ISP_Call SP_Call { get; }

        void Save();
    }
}
