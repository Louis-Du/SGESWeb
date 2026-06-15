namespace SGES.Web.Models
{
    public class UsuarioSesion
    {
        // ID del administrador o aprendiz
        public int Id { get; set; }

        // Nombre mostrado en la aplicación
        public string Nombre { get; set; }

        // "Administrador" o "Aprendiz"
        public string Tipo { get; set; }
    }
}