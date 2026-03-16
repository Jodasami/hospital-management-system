using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema_Hospitales.Models.viewModels
{
	public class mInventarioPrescripcion
	{
        [Required]
        [Display(Name = "Id Medicamento")]
        public int IdMedicamento { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Cantidad Disponible")]
        public int CantidadDisponible { get; set; }

        [Required]
        [Display(Name = "Cantidad Prescrita (30 Días)")]
        public int? CantidadPrescritaUltimos30Dias { get; set; }
    }
}