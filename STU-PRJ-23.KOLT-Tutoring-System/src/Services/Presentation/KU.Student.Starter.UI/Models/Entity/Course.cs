using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public List<CourseTutor>? Tutors { get; set; }
    }
}
