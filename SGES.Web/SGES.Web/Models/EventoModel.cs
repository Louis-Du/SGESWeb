using System;

namespace SGES.Web.Models
{
    public class EventoModel
    {
        public int IdEvento { get; set; }

        public string NombreEvento { get; set; }

        public string TipoEvento { get; set; }

        public DateTime DiaEvento { get; set; }

        public DateTime FechaHoraInicio { get; set; }

        public DateTime FechaHoraFin { get; set; }

        public int IdUser { get; set; }
    }
}