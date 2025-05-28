using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models
{
    public class DrawParticipant
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }
        public DateTime SubmitDate { get; set; }
        public bool IsStudent { get; set; }
        public string StudentId { get; set; }
        public bool IsWinner { get; set; }
    }
}
