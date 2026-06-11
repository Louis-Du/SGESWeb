using System;

namespace SGES.Web.Models
{
    public class EventoModel
    {
        public int IdEvento { get; set; }
        public string NombreEvento { get; set; }
        public string TipoEvento { get; set; }
        public string ModalidadEvento { get; set; }  // NUEVO
        public string TipoInscrip { get; set; }       // NUEVO
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public int IdUser { get; set; }
    }
}
