using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class EditScheduleTexts
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Main Ttle is required.")]
        public string MainTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "SubTitle is required.")]
        public string SubTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Below Table is required.")]
        public string BelowTable { get; set; } = string.Empty;

        [Required(ErrorMessage = "Page Title is required.")]
        public string PageTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Go to Top is required.")]
        public string GoToTop { get; set; } = string.Empty;

        [Required(ErrorMessage = "Not Published Text is required.")]
        public string NotPublishedText { get; set; } = string.Empty;
    }
}
