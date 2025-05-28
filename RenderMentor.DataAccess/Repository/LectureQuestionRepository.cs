using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System.Linq;

namespace RenderMentor.DataAccess.Repository
{
    public class LectureQuestionRepository : Repository<LectureQuestion>, ILectureQuestionRepository
    {
        private readonly ApplicationDbContext _db;

        public LectureQuestionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(LectureQuestion lectureQuestion)
        {
            _db.Update(lectureQuestion);
        }

    }
}
