using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class HeadTutor
    {
        [Key]
        public int Id { get; set; }
        public int HeadTutorsTutorId { get; set; }
        public int CourseId { get; set; }
        public int[] TutorIds { get; set; }
        public List<HeadTutorConnection>? Tutors { get; set; }

    }
}
