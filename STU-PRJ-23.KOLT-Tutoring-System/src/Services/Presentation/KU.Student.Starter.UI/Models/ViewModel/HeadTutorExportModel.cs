using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class HeadTutorExportModel
    {
        public string Course { get; set; } = string.Empty;
        public string HeadTutor { get; set; } = string.Empty;
        public string Tutors { get; set; } = string.Empty;
    }
}