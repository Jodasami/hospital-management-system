

using System.ComponentModel.DataAnnotations;

namespace Sistema_Hospitales.Models.viewModels
{
	public class mTratamiento
	{
        [Required]
        [Display(Name = "Id Tratamiento")]
        public int IdTratamiento { get; set; }

        [Required]
        [Display(Name = "Id Cita")]
        public int IdCita { get; set; }

        [Required]
        [Display(Name = "Costo Total")]
        public decimal CostoTotal { get; set; }

        public string NombrePaciente { get; set; }

        public string Medico { get; set; }
        public string Medicamento { get; set; }


    }
}