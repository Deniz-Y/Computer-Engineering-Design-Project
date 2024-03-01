using Microsoft.AspNetCore.Mvc.Rendering;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class HeadTutorViewModel
    {
        public int Id { get; set; }
        public int HeadTutorsTutorId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public Tutor HeadTutor { get; set; }
        public int[] TutorIds { get; set; }
        public string Tutors { get; set; }
        public List<SelectListItem> TutorOptions { get; set; }
        public List<SelectListItem> CourseOptions { get; set; }
    }
}