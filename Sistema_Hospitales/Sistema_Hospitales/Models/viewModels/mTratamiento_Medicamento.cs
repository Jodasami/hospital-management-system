

using System.ComponentModel.DataAnnotations;

namespace Sistema_Hospitales.Models.viewModels
{
	public class mTratamiento_Medicamento
	{
        [Required]
        [Display(Name = "Id Tratamiento")]
        public int IdTratamiento { get; set; }

        [Required]
        [Display(Name = "Id Medicamento")]
        public int IdMedicamento { get; set; }

        [Required]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        public string NombreMedicamento { get; set; }
    }
}