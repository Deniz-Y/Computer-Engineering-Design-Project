using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class TutorApplicationExportModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Courses { get; set; }
        public string GPA { get; set; } = string.Empty;
        public string? Periods { get; set; }
        public int WeeklyHour { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}