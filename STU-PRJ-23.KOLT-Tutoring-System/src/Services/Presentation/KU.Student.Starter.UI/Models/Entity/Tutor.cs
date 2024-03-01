using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class Tutor
    {
 
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string WeeklyHour { get; set; }
        public List<CourseTutor> Courses{ get; set; }
        public List<PeriodTutor> Periods { get; set; }
        public HeadTutorConnection HeadTutor { get; set; }
    }
}
