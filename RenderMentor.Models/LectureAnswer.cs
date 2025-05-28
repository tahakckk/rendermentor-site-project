using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RenderMentor.Models
{
    public class LectureAnswer
    {
        [Key]
        public int Id { get; set; }        
        public DateTime Date { get; set; }
        [Required]
        public string Answer { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public int LectureQuestionId { get; set; }
        [ForeignKey("LectureQuestionId")]
        public LectureQuestion LectureQuestion { get; set; }        

    }
}
