using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ILectureAnswerRepository : IRepository<LectureAnswer>
    {
        void Update(LectureAnswer lectureAnswer);
    }
}
