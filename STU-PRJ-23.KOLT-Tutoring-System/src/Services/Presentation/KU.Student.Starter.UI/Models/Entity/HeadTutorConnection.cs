using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class HeadTutorConnection
    {
        [Key]
        public int Id { get; set; }
        public int HeadTutorId { get; set; }
        public int[] TutorIds { get; set; }
        public List<Tutor>? Tutors { get; set; }
        public HeadTutor HeadTutor { get; set; }

    }
}
