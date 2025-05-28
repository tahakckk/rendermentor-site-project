using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class LectureAnswerRepository : Repository<LectureAnswer>, ILectureAnswerRepository
    {
        private readonly ApplicationDbContext _db;

        public LectureAnswerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(LectureAnswer lectureAnswer)
        {
            _db.Update(lectureAnswer);
        }

    }
}
