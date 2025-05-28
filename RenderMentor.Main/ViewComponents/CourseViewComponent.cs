using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RenderMentor.ViewComponents
{
    public class CourseViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke(string viewName = null)
        {
            var courses = _unitOfWork.Course.GetAll().OrderBy(i => i.ListOrder);

            foreach (var course in courses)
            {
                course.TotalDuration = new TimeSpan(_unitOfWork.CourseSection.GetAll().Where(n => n.CourseId == course.Id)
                        .Select(n => new CourseSection()
                        {
                            TotalDuration = new TimeSpan(_unitOfWork.Lecture.GetAll().Where(x => x.CourseSectionId == n.Id).Sum(x => x.Duration.Ticks))
                        }).Sum(m => m.TotalDuration.Ticks));
                course.TotalLectures = _unitOfWork.CourseSection.GetAll().Where(n => n.CourseId == course.Id)
                        .Select(n => new CourseSection()
                        {
                            TotalLectures = _unitOfWork.Lecture.GetAll().Where(x => x.CourseSectionId == n.Id).Count()
                        }).Sum(x => x.TotalLectures);
                course.CourseCovers = _unitOfWork.CourseCover.GetAll().Where(i => i.CourseId == course.Id).ToList();
                if (course.CourseCovers.Count() > 0)
                {
                    Random r = new Random();
                    int total = course.CourseCovers.Count();
                    int offset = r.Next(0, total);
                    var picked = course.CourseCovers.Skip(offset).FirstOrDefault();
                    course.CoverImage = picked.Image;
                }
                if (course.Instructor == null)
                {
                    course.Instructor = new Instructor()
                    {
                        ApplicationUser = new ApplicationUser()
                        {
                            Name = "Evren Çavuşoğlu"
                        }
                    };
                }
                else
                {
                    course.Instructor = _unitOfWork.Instructor.GetAll().Where(i => i.Id == course.InstructorId)
                        .Select(i => new Instructor()
                        {
                            ApplicationUser = _unitOfWork.ApplicationUser.GetAll().Where(n => n.Id == i.UserId)
                                .Select(n => new ApplicationUser()
                                {
                                    Name = n.Name
                                }).FirstOrDefault()
                        }).FirstOrDefault();
                }
            }

            if (!string.IsNullOrEmpty(viewName))
            {
                return View(viewName, courses);
            }
            return View(courses);
        }
    }
}
