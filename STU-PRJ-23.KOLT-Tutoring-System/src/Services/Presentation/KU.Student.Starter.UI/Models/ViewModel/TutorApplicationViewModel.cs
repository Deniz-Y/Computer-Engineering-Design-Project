using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class TutorApplicationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Courses { get; set; }
        public List<SelectListItem> CourseOptions { get; set; }
        [Display(Name = "Courses")]
        public int[] CourseIds { get; set; }
        public double GPA { get; set; }
        public string Periods { get; set; }
        public List<SelectListItem> PeriodOptions { get; set; }
        [Display(Name = "Periods")]
        public int[] PeriodIds { get; set; }
        public int WeeklyHour { get; set; }
        public string Status { get; set; }
    }
}