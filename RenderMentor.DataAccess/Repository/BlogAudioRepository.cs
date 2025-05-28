using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Repository
{
    public class BlogAudioRepository : Repository<BlogAudio>, IBlogAudioRepository
    {
        private readonly ApplicationDbContext _db;

        public BlogAudioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BlogAudio blogAudio)
        {
            var objFromDb = _db.BlogAudios.FirstOrDefault(s => s.Id == blogAudio.Id);
            if (objFromDb != null)
            {
                objFromDb.Audio = blogAudio.Audio;
                objFromDb.Title = blogAudio.Title;
                objFromDb.Desc = blogAudio.Desc;
                objFromDb.BlogId = blogAudio.BlogId;
            }
        }
    }
}
