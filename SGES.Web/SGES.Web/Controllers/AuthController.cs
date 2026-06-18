using SGES.Web.Models;
using System;
using System.Web.Mvc;

namespace SGES.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthDAO _dao;

        public AuthController() : this(new AuthDAO()) { }

        public AuthController(AuthDAO dao)
        {
            if (dao == null) throw new ArgumentNullException("dao");
            _dao = dao;
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Validar que solo contenga dígitos y tenga entre 1 y 10 caracteres
            if (!System.Text.RegularExpressions.Regex.IsMatch(model.Id, @"^\d{1,10}$"))
            {
                TempData["Error"] = "El ID debe contener solo números y tener máximo 10 dígitos.";
                return View(model);
            }

            // Convertir a int de forma segura
            if (!int.TryParse(model.Id, out int idNumerico))
            {
                TempData["Error"] = "El ID ingresado no es válido.";
                return View(model);
            }

            var usuario = _dao.Login(idNumerico, model.Contrasena);

            if (usuario == null)
            {
                TempData["Error"] = "ID o contraseña incorrectos.";
                return View(model);
            }

            Session["Usuario"] = usuario;
            return RedirigirSegunRol(usuario);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Logout()
        {
            Session["Usuario"] = null;
            Session.Clear();
            Session.Abandon();

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var cookie = new System.Web.HttpCookie("ASP.NET_SessionId");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            // Forzar request completamente nuevo con URL absoluta
            return Redirect(Url.Action("Login", "Auth", null, Request.Url.Scheme));
        }

        private ActionResult RedirigirSegunRol(UsuarioSesion usuario)
        {
            if (usuario.Tipo == "Administrador")
                return RedirectToAction("InicioAdmin", "Evento");

            return RedirectToAction("InicioAprendiz", "Evento");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult RestablecerPassword()
        {
            return View(new LoginModel());
        }

        // POST: /Auth/RestablecerPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RestablecerPassword(LoginModel model, string nuevaPassword, string confirmarPassword)
        {
            ModelState.Remove("Contrasena");

            if (!ModelState.IsValid)
                return View(model ?? new LoginModel());

            // Validar ID
            if (!System.Text.RegularExpressions.Regex.IsMatch(model.Id, @"^\d{1,10}$"))
            {
                TempData["Error"] = "El ID debe contener solo números y tener máximo 10 dígitos.";
                return View(model);
            }

            if (!int.TryParse(model.Id, out int idNumerico))
            {
                TempData["Error"] = "El ID ingresado no es válido.";
                return View(model);
            }

            var usuario = _dao.ObtenerUsuarioPorId(idNumerico);

            if (usuario == null)
            {
                TempData["Error"] = "El usuario no existe.";
                return View(model);
            }

            if (nuevaPassword != confirmarPassword)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(nuevaPassword))
            {
                TempData["Error"] = "La contraseña no puede estar vacía.";
                return View(model);
            }

            try
            {
                _dao.ActualizarPassword(usuario.Id, nuevaPassword, usuario.Tipo);
                TempData["Success"] = "Contraseña actualizada correctamente.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cambiar contraseña: " + ex.Message;
                return View(model);
            }
        }
    }
}
