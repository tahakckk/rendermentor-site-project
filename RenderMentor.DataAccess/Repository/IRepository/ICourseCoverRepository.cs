using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.DataAccess.Repository.IRepository
{
    public interface ICourseCoverRepository : IRepository<CourseCover>
    {
        void Update(CourseCover courseCover);
    }
}
