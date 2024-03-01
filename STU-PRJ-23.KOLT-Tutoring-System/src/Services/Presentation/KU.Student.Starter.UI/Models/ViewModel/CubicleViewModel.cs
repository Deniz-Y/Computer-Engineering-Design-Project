using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models.ViewModel
{
    public class CubicleViewModel
    {
        public int Id { get; set; }
        public string CubicleName { get; set; }
        public string CubicleNumber { get; set; }
        public string CubiclePlace { get; set; }
    }
}