using Microsoft.AspNetCore.Mvc.Rendering;
using RenderMentor.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderMentor.Models.ViewModels
{
    public class StudentIndexVM
    {
        public List<StudentCourseVM> StudentCourseVM { get; set; }

        public bool PackageActive { get; set; }
        public string PackageName { get; set; }
        public double PackageDaysToExpire { get; set; }

        public int PackageStatusId { get; set; }
        public PackageStatus PackageStatus
        {
            get => (PackageStatus)PackageStatusId;
            set => PackageStatusId = (int)value;
        }

        public int TrialStatusId { get; set; }
        public TrialStatus TrialStatus
        {
            get => (TrialStatus)TrialStatusId;
            set => TrialStatusId = (int)value;
        }
    }
}
