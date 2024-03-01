namespace KU.Student.Starter.UI.Models
{
    public class PeriodTutor
    {
        public int PeriodTutorId { get; set; }
        public int TutorId { get; set; }
        public int PeriodId { get; set; }
        public Tutor Tutor { get; set; }
        public Period Period { get; set; }
        public List<PeriodCubicle>? Cubicles { get; set; }
    }
}
