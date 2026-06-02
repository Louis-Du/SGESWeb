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
        public ActionResult Login()
        {
            // Si ya hay sesión activa, redirigir según rol
            if (Session["Usuario"] != null)
                return RedirigirSegunRol((UsuarioSesion)Session["Usuario"]);

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

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        private ActionResult RedirigirSegunRol(UsuarioSesion usuario)
        {
            if (usuario.Tipo == "Administrador")
                return RedirectToAction("CrearEvento", "Evento");

            return RedirectToAction("Listado", "Evento");
        }
    }
}
