namespace RenderMentor.Models
{
    // For join table between Instructors and CourseCategories (Many-to-many relationship)
    public class InstructorCategory
    {
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
        
        public int CourseCategoryId { get; set; }
        public CourseCategory CourseCategory { get; set; }

    }
}
