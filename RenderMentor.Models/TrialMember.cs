using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderMentor.Models
{
    public class TrialMember
    {
        [Key]
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string IPAddress { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
        public bool Started { get; set; }

        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}
