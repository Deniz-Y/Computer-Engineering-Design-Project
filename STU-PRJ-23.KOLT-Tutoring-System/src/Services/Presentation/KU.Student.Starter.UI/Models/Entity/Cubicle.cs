using System.ComponentModel.DataAnnotations;

namespace KU.Student.Starter.UI.Models
{
    public class Cubicle
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cubicle Name is required.")]
        public string CubicleName { get; set; }

        [Required(ErrorMessage = "Cubicle Number is required.")]
        public string CubicleNumber { get; set; }

        [Required(ErrorMessage = "Cubicle Place is required.")]
        public string CubiclePlace { get; set; }
        public List<PeriodCubicle>? PeriodTutors { get; set; }

    }
}
