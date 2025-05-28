using RenderMentor.Models;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ILectureQuestionRepository : IRepository<LectureQuestion>
    {
        void Update(LectureQuestion lectureQuestion);
    }
}
