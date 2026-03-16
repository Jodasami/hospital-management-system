using System;
using System.ComponentModel.DataAnnotations;


namespace Sistema_Hospitales.Models.viewModels
{
	public class mPago
	{
        [Required]
        [Display(Name = "Num Pago")]
        public int IdPago { get; set; }

        [Required]
        [Display(Name = "Cedula")]
        public string IdPaciente { get; set; }

        [Required]
        [Display(Name = "Id Tratamiento")]
        public int IdTratamiento { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Required]
        [Display(Name = "Metodo Pago")]
        public string MetodoPago { get; set; }

        [Required]
        [Display(Name = "Pagado")]
        public bool Pagado { get; set; }

        public string Medicamento { get; set; }
        public string Paciente { get; set; }
    }
}