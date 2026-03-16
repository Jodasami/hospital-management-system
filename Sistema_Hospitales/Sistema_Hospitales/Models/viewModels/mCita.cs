using System;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Hospitales.Models.viewModels
{
	public class mCita
	{
        [Required]
        [Display(Name = "Id Cita")]
        public int IdCita { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }

        [Required]
        [Display(Name = "Hora")]
        public TimeSpan? Hora { get; set; }

        [Required]
        [Display(Name = "Id Medico")]
        public int? IdMedico { get; set; }

        [Required]
        [Display(Name = "Id Paciente")]
        public string IdPaciente { get; set; }

        [Required]
        [Display(Name = "Id Hospital")]
        public int? IdHospital { get; set; }

        [Required]
        [Display(Name = "Diagnostico")]
        [StringLength(500)]
        public string Diagnostico { get; set; }

        public string Medico { get; set; }
        public string Hospital { get; set; }
        public string Paciente { get; set;}  

    }
}