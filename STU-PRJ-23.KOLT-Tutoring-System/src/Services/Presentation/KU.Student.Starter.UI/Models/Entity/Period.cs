using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class Period
    {
        [Key]
        public int Id { get; set; }
        public string Day { get; set; }

        [Required(ErrorMessage = "Start Hour is required")]
        public string StartHour { get; set; }

        [Required(ErrorMessage = "End Hour is required")]
        public string EndHour { get; set; }
        public List<PeriodTutor>? Tutors { get; set; }
    }
}
