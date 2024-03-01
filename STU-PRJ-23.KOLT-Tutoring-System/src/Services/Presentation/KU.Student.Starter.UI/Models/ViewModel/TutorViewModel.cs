using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class TutorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string WeeklyHour { get; set; }
        public string Courses { get; set; }
        public List<SelectListItem> CourseOptions { get; set; }
        //public List<Course> AllCourses { get; set; }
        [Display(Name = "Courses")]
        public int[] CourseIds { get; set; }
        public string Periods { get; set; }
        public List<SelectListItem> PeriodOptions { get; set; }
       // public List<Period> AllPeriods { get; set; }
        [Display(Name = "Periods")]
        public int[] PeriodIds { get; set; }

    }
}