using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ICourseListRepository : IRepository<CourseList>
    {
        void Update(CourseList courseList);
    }
}
