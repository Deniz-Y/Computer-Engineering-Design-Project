using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class ConfigurationViewModel
    {
        public int Id { get; set; }
        public bool PublishSchedule { get; set; }
        public String Date { get; set; } = String.Empty;
        public String Time { get; set; } = String.Empty;
        public int NumberOfCubicles { get; set; }
        public int ScheduleSplitCount { get; set; }
        public String UserName { get; set; } = String.Empty;
        public String EditDateTime { get; set; } = String.Empty;

    }
}