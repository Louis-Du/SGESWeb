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

        // GET: /Evento/AprendicesRegistrados?idEvento=5
        public ActionResult AprendicesRegistrados(int? idEvento)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");
            if (idEvento == null)
                return new HttpStatusCodeResult(400, "idEvento requerido");

            try
            {
                List<AprendizModel> inscritos = _dao.ObtenerAprendicesPorEvento(idEvento.Value);
                return View(inscritos ?? new List<AprendizModel>());
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error al consultar aprendices: " + ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // GET: /Evento/ModificarEvento?id=5
        // Recibe el id del evento seleccionado en InicioAdmin y carga el formulario
        // con los datos actuales del evento para que el admin los edite.
        // ─────────────────────────────────────────────────────────────────────────
        [HttpGet]
        public ActionResult ModificarEvento(int? id)
        {
            // Si no hay sesión activa, redirigir al login
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            // Si no se recibió ningún id (el admin no seleccionó evento), volver al inicio
            if (id == null)
            {
                TempData["Error"] = "Debe seleccionar un evento para modificar.";
                return RedirectToAction("InicioAdmin");
            }

            // Buscamos el evento en la BD por su id
            var evento = _dao.ObtenerEventos().FirstOrDefault(e => e.IdEvento == id.Value);

            // Si no existe el evento con ese id, volver al inicio con mensaje
            if (evento == null)
            {
                TempData["Error"] = "El evento seleccionado no existe.";
                return RedirectToAction("InicioAdmin");
            }

            // Cargamos los tipos de evento para el ComboBox
            ViewBag.TiposEvento = ObtenerTiposEvento();

            // Enviamos el evento a la vista con sus datos actuales precargados
            return View(evento);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST: /Evento/ModificarEvento
        // Recibe el formulario con los datos editados, aplica validaciones
        // y si todo está bien actualiza el evento en la BD.
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificarEvento(EventoModel evento)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            // Siempre recargamos el ComboBox antes de cualquier return View()
            ViewBag.TiposEvento = ObtenerTiposEvento();

            // ── VALIDACIÓN 1: Fecha de inicio no puede ser en el pasado ──────────
            var ahora = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, 0
            );

            if (evento.FechaHoraInicio < ahora)
            {
                ModelState.AddModelError("FechaHoraInicio",
                    "La fecha de inicio no puede ser anterior a la fecha y hora actual.");
            }

            // ── VALIDACIÓN 2: Fecha fin debe ser posterior a fecha inicio ─────────
            if (evento.FechaHoraFin != default(DateTime)
                && evento.FechaHoraInicio != default(DateTime)
                && evento.FechaHoraFin <= evento.FechaHoraInicio)
            {
                ModelState.AddModelError("FechaHoraFin",
                    "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            // ── VALIDACIÓN 3: Duración mínima de 30 minutos ───────────────────────
            if (evento.FechaHoraFin != default(DateTime)
                && evento.FechaHoraInicio != default(DateTime)
                && (evento.FechaHoraFin - evento.FechaHoraInicio).TotalMinutes < 30)
            {
                ModelState.AddModelError("FechaHoraFin",
                    "La duración del evento debe ser de al menos 30 minutos.");
            }

            // Si alguna validación falló, devolvemos la vista con los errores
            if (!ModelState.IsValid)
                return View(evento);

            try
            {
                // Actualizamos el evento en la BD
                _dao.ActualizarEvento(evento);
                TempData["Success"] = "Evento actualizado correctamente.";
                return RedirectToAction("InicioAdmin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar el evento: " + ex.Message);
                return View(evento);
            }
        }

    }
}
