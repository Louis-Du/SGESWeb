using System;

namespace SGES.Web.Models
{
    // Modelo simple para mostrar aprendices inscritos
    public class AprendizModel
    {
        public int IdApr { get; set; }
        public string NombreApr { get; set; }
        public string EmailApr { get; set; }     // campo real independiente
        public string ContactoApr { get; set; }  // campo real independiente
        public string Ficha { get; set; }
    }
}