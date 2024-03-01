using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class SessionCubicleTableViewModel
    {
        [Key]
        public List<Cubicle> Cubicles { get; set; }
        public List<Period> Periods { get; set; }
        public List<PeriodCubicle> PeriodCubicles { get; set; }
        public List<CourseTutor> TutorCourses { get; internal set; }
        public List<Course> Courses { get; internal set; }
        public string Day { get; set; }
    }
}
