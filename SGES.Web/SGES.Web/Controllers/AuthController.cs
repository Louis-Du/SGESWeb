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

            var usuario = _dao.Login(model.Id, model.Contrasena);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "ID o contraseña incorrectos.");
                return View(model);
            }

            // Guardar en sesión
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
    }
}
