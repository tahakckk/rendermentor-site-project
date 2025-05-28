using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IBlogAuthorRepository : IRepository<BlogAuthor>
    {
        void Update(BlogAuthor blogAuthor);
    }
}
