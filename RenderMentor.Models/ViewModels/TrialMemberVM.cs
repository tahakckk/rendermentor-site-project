using System;
using System.ComponentModel.DataAnnotations;
using static RenderMentor.Models.ApplicationUser;

namespace RenderMentor.Models.ViewModels
{
    public class TrialMemberVM
    {
        public DateTime StartDate { get; set; }
        public Student Student { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string TrialStatus { get; set; }
    }
}
