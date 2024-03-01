using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class CouselyScheduleTable
    {
        public int CourseId { get; internal set; }
        public string CourseCode { get; set; }
        public List<RowsCouselyScheduleTable> Rows { get; internal set; }
        public List<RowsCouselyScheduleTable> SortedRows { get; internal set; }
    }
}