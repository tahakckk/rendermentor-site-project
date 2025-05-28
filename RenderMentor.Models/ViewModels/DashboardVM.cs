using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RenderMentor.Models.ViewModels
{
    public class DashboardVM
    {
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCompanies { get; set; }
    }
}
