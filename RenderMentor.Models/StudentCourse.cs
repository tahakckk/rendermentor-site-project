namespace RenderMentor.Models
{
    // For join table between Students and Courses (Many-to-many relationship)
    public class StudentCourse
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public bool isPaid { get; set; }

        public int WatchingLectureId { get; set; }
    }
}
