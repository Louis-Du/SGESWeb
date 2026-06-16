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

            // Validar formato: solo dígitos, sin ceros a la izquierda (salvo "0")
            if (!System.Text.RegularExpressions.Regex.IsMatch(model.Id, @"^(0|[1-9][0-9]*)$"))
            {
                TempData["Error"] = "ID o contraseña incorrectos.";
                return View(model);
            }

            int idNumerico = int.Parse(model.Id);

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
            if (model == null || !ModelState.IsValid)
                return View(model ?? new LoginModel());

            // Validar formato: solo dígitos, sin ceros a la izquierda (salvo "0")
            if (!System.Text.RegularExpressions.Regex.IsMatch(model.Id, @"^(0|[1-9][0-9]*)$"))
            {
                TempData["Error"] = "El usuario no existe.";
                return View(model);
            }

            int idNumerico = int.Parse(model.Id);
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
