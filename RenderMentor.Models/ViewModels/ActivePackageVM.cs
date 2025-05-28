using System;
using System.ComponentModel.DataAnnotations;
using static RenderMentor.Models.ApplicationUser;

namespace RenderMentor.Models.ViewModels
{
    public class ActivePackageVM
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PackageName { get; set; }
        public double DaysToExpire { get; set; }
        public Student Student { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string UserId { get; set; }
    }
}
