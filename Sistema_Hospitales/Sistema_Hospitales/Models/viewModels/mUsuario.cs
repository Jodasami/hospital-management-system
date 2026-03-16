using System.ComponentModel.DataAnnotations;


namespace Sistema_Hospitales.Models.viewModels
{
	public class mUsuario
	{
        public int IdUsuario { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public int IdRol { get; set; }

        [Display(Name = "Cedula")]
        public string IdPaciente { get; set; }

        [Display(Name = "Medico")]
        public int? IdMedico { get; set; }
    }
}