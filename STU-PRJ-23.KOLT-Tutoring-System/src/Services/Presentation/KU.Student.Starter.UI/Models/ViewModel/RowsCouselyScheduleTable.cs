using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class RowsCouselyScheduleTable
    {
        public string Day { get; internal set; }
        public string Time { get; internal set; }
        public string Place { get; internal set; }
        public string Tutor { get; internal set; }
        public string CubicleNo { get; internal set; }
        public string StartHour { get; internal set; }
    }
}