namespace KU.Student.Starter.UI.Models
{
    public class PeriodCubicle
    {
        public int PeriodCubicleId { get; set; }
        public int PeriodTutorId { get; set; }
        public int CubicleId { get; set; }
        public PeriodTutor PeriodTutor { get; set; }
        public Cubicle Cubicle { get; set; }
    }
}
