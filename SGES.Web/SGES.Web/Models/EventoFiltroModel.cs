using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGES.Web.Models
{
    public class EventoFiltroModel
    {
        public string Nombre { get; set; }
        public DateTime? Fecha { get; set; }  // nullable, vacío = sin filtro
        public string Modalidad { get; set; }
        public string TipoInscrip { get; set; }
    }
}