using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SGES.Web.Models
{
    // InscripcionModel representa los datos que se necesitan
    // para registrar un aprendiz en un evento.
    // Es el "formulario" que viaja entre la vista y el controlador.
    public class InscripcionModel
    {
        // ID del evento al que se quiere inscribir.
        // Se pasa como campo oculto desde la vista (no lo escribe el usuario).
        [Required]
        public int IdEvento { get; set; }

        // ID del aprendiz. Se toma de la sesión activa en el controlador
        public int IdApr { get; set; }

        // Modalidad elegida por el aprendiz: Presencial o Virtual.
        [Required(ErrorMessage = "Seleccionar la modalidad es obligatorio.")]
        public string Modalidad { get; set; }

        // Fecha del día en que se realizó la inscripción.
        // Se asigna automáticamente en el controlador con DateTime.Today.
        public DateTime FechaInscrip { get; set; }
    }
}