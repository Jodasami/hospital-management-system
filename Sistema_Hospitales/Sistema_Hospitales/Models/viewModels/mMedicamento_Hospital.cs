

using System.ComponentModel.DataAnnotations;

namespace Sistema_Hospitales.Models.viewModels
{
	public class mMedicamento_Hospital
	{
        [Required]
        [Display(Name = "Id Hospital")]
        public int IdHospital { get; set; }

        [Required]
        [Display(Name = "Id Medicamento")]
        public int IdMedicamento { get; set; }

        [Required]
        [Display(Name = "Cantidad Disponible")]
        public int CantidadDisponible { get; set; }

        public string Hospital {  get; set; }
        public string Medicamento {  get; set; }
    }
}