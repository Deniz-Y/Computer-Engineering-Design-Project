using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class WeeklyViewModel
    {
        public List<Period> Periods { get; internal set; }
        public Dictionary<string, int> MondayHours { get; internal set; }
        public Dictionary<string, int> TuesdayHours { get; internal set; }
        public Dictionary<string, int> WednesdayHours { get; internal set; }
        public Dictionary<string, int> ThursdayHours { get; internal set; }
        public Dictionary<string, int> FridayHours { get; internal set; }
    }
}