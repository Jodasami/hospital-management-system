

using System.ComponentModel.DataAnnotations;

namespace Sistema_Hospitales.Models.viewModels
{
	public class mMedicamento
	{
        [Required]
        [Display(Name = "Id Medicamento")]
        public int IdMedicamento { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "Costo x Unidad")]
        public decimal CostoUnidad { get; set; }
    }
}