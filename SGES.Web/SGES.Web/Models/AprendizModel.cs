using System;

namespace SGES.Web.Models
{
    // Modelo simple para mostrar aprendices inscritos
    public class AprendizModel
    {
        public int IdApr { get; set; }
        public string NombreApr { get; set; }

        // Campo principal para contacto/email (llenado por DAO)
        public string Email { get; set; }

        // Alias por compatibilidad: algunas vistas/DAOs usaban ContactoApr o EmailApr
        public string ContactoApr
        {
            get => Email;
            set => Email = value;
        }

        // Alias adicional por compatibilidad si existe código que esperaba EmailApr
        public string EmailApr
        {
            get => Email;
            set => Email = value;
        }
            
        public string Ficha { get; set; } // opcional
    }
}