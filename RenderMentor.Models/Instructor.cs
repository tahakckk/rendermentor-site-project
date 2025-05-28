using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderMentor.Models
{
    public class Instructor
    {
        [Key]
        public int Id { get; set; }
        public string Desc { get; set; }
        public string AvatarImage { get; set; }
        public int ListOrder { get; set; }

        public List<InstructorCategory> InstructorCategories { get; set; } = new List<InstructorCategory>();

        public ICollection<Course> Courses { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public bool IsAdmin { get; set; }
    }
}
