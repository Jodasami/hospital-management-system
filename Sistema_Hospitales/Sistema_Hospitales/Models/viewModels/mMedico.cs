

using System.ComponentModel.DataAnnotations;

namespace Sistema_Hospitales.Models.viewModels
{
	public class mMedico
	{
        [Required]
        [Display(Name = "Id Medico")]
        public int IdMedico { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Especialidad")]
        public string Especialidad { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Id Hospital")]
        public int IdHospital { get; set; }

        public string NombreHospital { get; set; }

    }
}