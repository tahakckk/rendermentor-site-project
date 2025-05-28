using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface IBlogThumbRepository : IRepository<BlogThumb>
    {
        void Update(BlogThumb blogThumb);
    }
}
