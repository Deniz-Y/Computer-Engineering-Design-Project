using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class Configuration
    {
        [Key]
        public int Id { get; set; }
        public bool PublishSchedule { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public String Date { get; set; } = String.Empty;

        [Required(ErrorMessage = "Time is required.")]
        public String Time { get; set; } = String.Empty;
        public int NumberOfCubicles { get; set; }
        public int ScheduleSplitCount { get; set; }
        public String UserName { get; set; } = String.Empty;
        public String EditDateTime { get; set; } = String.Empty;

    }
}
