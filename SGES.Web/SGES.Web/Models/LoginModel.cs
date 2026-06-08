using System.ComponentModel.DataAnnotations;

namespace SGES.Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Contrasena { get; set; }
    }
}
