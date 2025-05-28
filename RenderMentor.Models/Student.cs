using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderMentor.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public bool SubscribeActive { get; set; }
        public bool SubscribeStarted { get; set; }
        public DateTime SubscribeStartDate { get; set; }
        public DateTime SubscribeExpireDate { get; set; }
    }
}
