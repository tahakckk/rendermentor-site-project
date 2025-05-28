using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class BlogAuthorRepository : Repository<BlogAuthor>, IBlogAuthorRepository
    {
        private readonly ApplicationDbContext _db;

        public BlogAuthorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(BlogAuthor blogAuthor)
        {
            _db.Update(blogAuthor);
        }

    }
}
