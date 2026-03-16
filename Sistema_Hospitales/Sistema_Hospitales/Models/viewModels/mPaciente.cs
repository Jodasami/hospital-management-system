using System;
using System.ComponentModel.DataAnnotations;


namespace Sistema_Hospitales.Models.viewModels
{
	public class mPaciente
	{
        [Required]
        [Display(Name = "Cedula")]
        public string IdPaciente { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [Display(Name = "Genero")]
        public string Genero { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [Display(Name = "Telefono")]
        public string Telefono { get; set; }

        [Required]
        [Display(Name = "Id Hospital")]
        public int IdHospital { get; set; }

        public string NombreHospital { get; set; }

    }
}