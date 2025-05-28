using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ICourseReviewRepository : IRepository<CourseReview>
    {
        void Update(CourseReview courseReview);
    }
}
