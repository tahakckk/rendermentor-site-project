using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    class LectureRepository : Repository<Lecture>, ILectureRepository
    {
        private readonly ApplicationDbContext _db;

        public LectureRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Lecture lecture)
        {
            var objFromDb = _db.Lectures.FirstOrDefault(s => s.Id == lecture.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = lecture.Title;
                objFromDb.Video = lecture.Video;
                objFromDb.IsPreview = lecture.IsPreview;
                objFromDb.Description = lecture.Description;
                objFromDb.MainDescription = lecture.MainDescription;
                objFromDb.Duration = lecture.Duration;
                objFromDb.CourseSectionId = lecture.CourseSectionId;
            }
        }
    }
}
