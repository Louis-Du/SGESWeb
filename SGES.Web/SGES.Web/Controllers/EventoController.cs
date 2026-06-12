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

        private SelectList ObtenerModalidades()
        {
            return new SelectList(new List<string>
            {
                "Presencial",
                "Virtual"
            });
        }

        private SelectList ObtenerTiposInscripcion()
        {
            return new SelectList(new List<string>
            {
               "Individual",
               "Grupal"
            });
        }

        [HttpGet]
        public ActionResult CrearEvento()
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            ViewBag.TiposEvento = ObtenerTiposEvento();
            ViewBag.Modalidades = ObtenerModalidades();
            ViewBag.TiposInscripcion = ObtenerTiposInscripcion();

            return View(new EventoModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEvento(EventoModel evento)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            ViewBag.TiposEvento = ObtenerTiposEvento();
            ViewBag.Modalidades = ObtenerModalidades();       
            ViewBag.TiposInscripcion = ObtenerTiposInscripcion();

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

            if (evento.FechaHoraFin != default(DateTime)
                && evento.FechaHoraInicio != default(DateTime)
                && evento.FechaHoraFin > evento.FechaHoraInicio
                && (evento.FechaHoraFin - evento.FechaHoraInicio).TotalMinutes < 30)
            {
                ModelState.AddModelError(
                    "FechaHoraFin",
                    "La duración del evento debe ser de al menos 30 minutos."
                );
            }

            // Validación cupo máximo
            if (evento.TipoInscrip == "Grupal" && evento.CupoMaximo <= 0)
            {
                ModelState.AddModelError("CupoMaximo",
                    "Para eventos grupales debe indicar un cupo máximo mayor a cero.");
            }
            if (evento.TipoInscrip == "Individual")
            {
                evento.CupoMaximo = 0; // forzar a 0 por si el JS falló
            }

            if (!ModelState.IsValid)
                return View(evento);

            try
            {
                _dao.InsertarEvento(evento);
                TempData["Success"] = "Evento creado correctamente.";
                return RedirectToAction("InicioAdmin");
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
            .OrderBy(e => e.FechaHoraInicio < DateTime.Now) // pasados al final
            .ThenBy(e => e.FechaHoraInicio)
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
                TempData["Error"] = "Ya estas inscrito en este evento.";
                return RedirectToAction("InicioAprendiz");
            }

            // pasamos la lista de modalidades al combobox de la vista.
            ViewBag.Modalidades = ObtenerModalidades();
            ViewBag.TiposInscripcion = ObtenerTiposInscripcion();
            ViewBag.Evento = evento;

            // preparamos el modelo para el formulario de inscripción con el id del evento ya cargado.
            var inscripcion = new InscripcionModel { IdEvento = id };
            return View(inscripcion);
        }

        // ───────────────────────────────────────────────────────────────────
        // POST: /Evento/Details
        // Procesa el formulario de inscripción enviado por el aprendiz.
        // ────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(InscripcionModel inscripcion)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            // Recuperamos el evento para mostrarlo de nuevo si hay error.
            var evento = _dao.ObtenerEventos().FirstOrDefault(e => e.IdEvento == inscripcion.IdEvento);

            if (evento.CupoMaximo > 0)
            {
                int inscritos = _inscripcionDao.ContarInscritos(inscripcion.IdEvento);
                if (inscritos >= evento.CupoMaximo)
                {
                    ModelState.AddModelError(string.Empty,
                        "Este evento ya no tiene cupos disponibles.");
                    ViewBag.Modalidades = ObtenerModalidades();
                    ViewBag.TiposInscripcion = ObtenerTiposInscripcion();
                    ViewBag.Evento = evento;
                    return View(inscripcion);
                }
            }

            // tomamos el ID del aprendiz desde la sesión, no del formulario (para evitar manipulación).
            inscripcion.IdApr = UsuarioActual.Id;

            // asignamos la fecha de inscripción al momento actual.
            inscripcion.FechaInscrip = DateTime.Today;

            // --- VALIDACION 1: ¿Ya está inscrito en este evento? ------------------
            if (_inscripcionDao.YaInscrito(inscripcion.IdApr, inscripcion.IdEvento))
            {
                ModelState.AddModelError(string.Empty,
                    "Ya estás inscrito en este evento.");

                ViewBag.Modalidades = ObtenerModalidades();
                ViewBag.TiposInscripcion = ObtenerTiposInscripcion();
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

                    return RedirectToAction("InicioAdmin");
                }

                _dao.EliminarEventos(idEvento);

                TempData["Success"] = "Evento eliminado correctamente.";

                return RedirectToAction("InicioAdmin");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error al eliminar el evento: " + ex.Message;

                return RedirectToAction("InicioAdmin");
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

                return RedirectToAction("InicioAdmin");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error al eliminar el evento: " + ex.Message;

                return RedirectToAction("InicioAdmin");
            }
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
            ViewBag.Modalidades = ObtenerModalidades();
            ViewBag.TiposInscripcion = ObtenerTiposInscripcion();

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
            ViewBag.Modalidades = ObtenerModalidades();
            ViewBag.TiposInscripcion = ObtenerTiposInscripcion();

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

            // Validación cupo máximo
            if (evento.TipoInscrip == "Grupal" && evento.CupoMaximo <= 0)
            {
                ModelState.AddModelError("CupoMaximo",
                    "Para eventos grupales debe indicar un cupo máximo mayor a cero.");
            }
            if (evento.TipoInscrip == "Individual")
            {
                evento.CupoMaximo = 0; // forzar a 0 por si el JS falló
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