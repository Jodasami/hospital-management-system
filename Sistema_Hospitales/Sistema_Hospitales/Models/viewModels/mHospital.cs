using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema_Hospitales.Models.viewModels
{
	public class mHospital
	{
        [Required]
        [Display(Name = "Id Hospital")]
        public int IdHospital { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [Display(Name = "Num Teléfono")]
        public string Telefono { get; set; }
    }
}