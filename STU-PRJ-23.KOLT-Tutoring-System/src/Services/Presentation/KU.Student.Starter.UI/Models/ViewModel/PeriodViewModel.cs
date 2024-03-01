using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class PeriodViewModel
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public string? Tutors { get; set; }
        public List<SelectListItem> DayOptions { get; set; }
    }
}