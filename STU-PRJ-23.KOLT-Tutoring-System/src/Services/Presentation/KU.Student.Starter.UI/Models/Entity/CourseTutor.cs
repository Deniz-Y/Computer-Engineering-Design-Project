namespace KU.Student.Starter.UI.Models
{
    public class CourseTutor
    {

        public int TutorId { get; set; }
        public int CourseId { get; set; }
        public Tutor Tutor { get; set; }
        public Course Course { get; set; }
    }
}
