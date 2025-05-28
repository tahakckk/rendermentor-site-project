using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ICourseRepository : IRepository<Course>
    {
        void Update(Course course);
    }
}
