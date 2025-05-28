using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IBlogRepository : IRepository<Blog>
    {
        void Update(Blog blog);
    }
}
