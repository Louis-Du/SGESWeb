using SGES.Web.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SGES.Web.Controllers
{
    public class EventoController : Controller
    {
        private readonly EventoDAO _dao;

        // DAO de inscripciones, separado del DAO de eventos
        // para respetar el principio de responsabilidad única.
        private readonly InscripcionDAO _inscripcionDao = new InscripcionDAO();

        public EventoController() : this(new EventoDAO()) { }

        public EventoController(EventoDAO dao)
        {
            if (dao == null) throw new ArgumentNullException("dao");
            _dao = dao;
        }

        // Helper: obtener usuario de sesión, redirigir a login si no hay
        private UsuarioSesion UsuarioActual
        {
            get { return Session["Usuario"] as UsuarioSesion; }
        }

        private SelectList ObtenerTiposEvento()
        {
            return new SelectList(new List<string>
            {
                "Educativo",
                "Deportivo",
                "Social",
                "Cultural"
            });
        }

        [HttpGet]
        public ActionResult CrearEvento()
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            ViewBag.TiposEvento = ObtenerTiposEvento();
            return View(new EventoModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEvento(EventoModel evento)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            ViewBag.TiposEvento = ObtenerTiposEvento();

            // Asignar el idUser desde la sesión
            evento.IdUser = UsuarioActual.Id;

            var ahora = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, 0
            );

            if (evento.FechaHoraInicio < ahora)
            {
                ModelState.AddModelError(
                    "FechaHoraInicio",
                    "La fecha de inicio no puede ser anterior a la fecha y hora actual."
                );
            }

            if (evento.FechaHoraFin != default(DateTime)
                && evento.FechaHoraInicio != default(DateTime)
                && evento.FechaHoraFin <= evento.FechaHoraInicio)
            {
                ModelState.AddModelError(
                    "FechaHoraFin",
                    "La fecha de fin debe ser posterior a la fecha de inicio."
                );
            }

            if (!ModelState.IsValid)
                return View(evento);

            try
            {
                _dao.InsertarEvento(evento);
                TempData["Success"] = "Evento creado correctamente.";
                return RedirectToAction("CrearEvento");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar el evento: " + ex.Message);
                return View(evento);
            }
        }

        [HttpGet]
        public ActionResult Listado()
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            var eventos = _dao.ObtenerEventos() ?? new List<EventoModel>();

            var disponibles = eventos
                .Where(e => e.FechaHoraInicio >= DateTime.Now)
                .OrderBy(e => e.FechaHoraInicio)
                .ToList();

            return View(disponibles);
        }

        [HttpGet]
        public ActionResult InicioAdmin()
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            var eventos = _dao.ObtenerEventos() ?? new List<EventoModel>();

            var disponibles = eventos
                .Where(e => e.FechaHoraInicio >= DateTime.Now)
                .OrderBy(e => e.FechaHoraInicio)
                .ToList();

            return View(disponibles);
        }

        [HttpPost]
        public ActionResult Eliminar(int idEvento)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                int cantidadInscritos = _inscripcionDao.VerificarInscritos(idEvento);

                if (cantidadInscritos > 0)
                {
                    TempData["ConfirmarEliminar"] = true;
                    TempData["EventoId"] = idEvento;
                    TempData["Mensaje"] =
                        "Este evento tiene aprendices inscritos. ¿Desea eliminarlos también?";

                    return RedirectToAction("Listado");
                }

                _dao.EliminarEventos(idEvento);

                TempData["Success"] = "Evento eliminado correctamente.";

                return RedirectToAction("Listado");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error al eliminar el evento: " + ex.Message;

                return RedirectToAction("Listado");
            }
        }

        [HttpPost]
        public ActionResult EliminarConfirmado(int idEvento)
        {
            try
            {
                if (UsuarioActual == null)
                    return RedirectToAction("Login", "Auth");

                _inscripcionDao.EliminarInscritos(idEvento);

                _dao.EliminarEventos(idEvento);

                TempData["Success"] =
                    "Evento e inscripciones eliminados correctamente.";

                return RedirectToAction("Listado");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error al eliminar el evento: " + ex.Message;

                return RedirectToAction("Listado");
            }
        }
    }
}
