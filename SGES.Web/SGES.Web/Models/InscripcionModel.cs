using System;
using System.ComponentModel.DataAnnotations;

namespace SGES.Web.Models
{
    public class InscripcionModel
    {
        public int IdInscrip { get; set; }

        public int IdEvento { get; set; }

        public int IdApr { get; set; }

        public DateTime FechaInscrip { get; set; }

        [Required(ErrorMessage = "Seleccionar la modalidad es obligatorio.")]
        public string Modalidad { get; set; }
    }
}