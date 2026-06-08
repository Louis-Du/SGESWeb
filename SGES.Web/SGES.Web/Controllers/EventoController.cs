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
        public ActionResult InicioAprendiz()
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

        // ─────────────────────────────────────────────────────────────────────
        // GET: /Evento/Details/5
        // Muestra el detalle de un evento con el formulario de inscripción.
        // Recibe el id del evento por la URL (RouteConfig: {controller}/{action}/{id}).
        // ───────────────────────────────────────────────────────────
        [HttpGet]
        public ActionResult Details(int id)
        {
            // Si no hay sesion activa, redirigir a login.
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            //se busca el evento por su id en la lista completa de eventos.
            var evento = _dao.ObtenerEventos().FirstOrDefault(e => e.IdEvento == id);

            // si el evento no existe, redirigir al InicioAprendiz con mensaje de error.
            if (evento == null)
            {
                TempData["Error"] = "Evento no encontrado.";
                return RedirectToAction("InicioAprendiz");
            }


            // VALIDACIÓN: si el aprendiz ya está inscrito en este evento,
            // no tiene sentido mostrar el formulario — lo devolvemos al inicio
            // con un mensaje informativo.
            if (_inscripcionDao.YaInscrito(UsuarioActual.Id, id))
            {
                TempData["Error"] = "Ya estás inscrito en este evento.";
                return RedirectToAction("InicioAprendiz");
            }

            // pasamos la lista de modalidades al combobox de la vista.
            ViewBag.Modalidades = new SelectList(new List<string> { "Presencial", "Virtual" });
            ViewBag.Evento = evento;

            // preparamos el modelo para el formulario de inscripción con el id del evento ya cargado.
            var inscripcion = new InscripcionModel { IdEvento = id };
            return View(inscripcion);
        }

        // ───────────────────────────────────────────────────────────────────
        // POST: /Evento/Details
        // Procesa el formulario de inscripción enviado por el aprendiz.
        // ────────────────────────────────────────────────────────────────────-
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(InscripcionModel inscripcion)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            // Recuperamos el evento para mostrarlo de nuevo si hay error.
            var evento = _dao.ObtenerEventos().FirstOrDefault(e => e.IdEvento == inscripcion.IdEvento);

            // tomamos el ID del aprendiz desde la sesión, no del formulario (para evitar manipulación).
            inscripcion.IdApr = UsuarioActual.Id;

            // asignamos la fecha de inscripción al momento actual.
            inscripcion.FechaInscrip = DateTime.Today;

            // --- VALIDACION 1: ¿Ya está inscrito en este evento? ------------------
            if (_inscripcionDao.YaInscrito(inscripcion.IdApr, inscripcion.IdEvento))
            {
                ModelState.AddModelError(string.Empty,
                    "Ya estás inscrito en este evento.");

                ViewBag.Modalidades = new SelectList(new List<string> { "Presencial", "Virtual" });
                ViewBag.Evento = evento;

                return View(inscripcion);
            }

            // --- VALIDACION 2: ¿hay cruce de horario con otro evento? --------------
            if (_inscripcionDao.TieneCruceDeHorario(inscripcion.IdApr, inscripcion.IdEvento))
            {
                ModelState.AddModelError(string.Empty,
                    "No puedes inscribirte porque tienes otro evento que se cruza en horario.");
                return View(inscripcion);
            }


            // --- VALIDACIÓN 3: ModelState (campos requeridos, etc.) -------------
            if (!ModelState.IsValid)
            {
                return View(inscripcion);
            }

            // Si pasó todas las validaciones, guardamos la inscripción.
            try
            {
                _inscripcionDao.Inscribir(inscripcion);
                // TempData persiste solo hasta la siguiente petición (el redirect).
                TempData["Success"] = "Inscripción realizada correctamente.";
                return RedirectToAction("InicioAprendiz");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al inscribirse: " + ex.Message);
                return View(inscripcion);
            }

        }
    }
}
