using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class CombinedCourseViewModel
    {
        public List<CouselyScheduleTable> Courselies { get; internal set; }
        public List<CouselyScheduleTable> SortedCourselies { get; internal set; }
        public EditScheduleTexts Texts { get; internal set; }
        public bool Publish { get; internal set; }
    }
}