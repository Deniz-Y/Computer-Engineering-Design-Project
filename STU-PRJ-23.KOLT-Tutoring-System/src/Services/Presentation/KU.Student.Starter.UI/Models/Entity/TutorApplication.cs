using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class TutorApplication
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public double GPA { get; set; }
        public int[] CourseIds { get; set; }
        public int[] PeriodIds { get; set; }
        public int WeeklyHour { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
