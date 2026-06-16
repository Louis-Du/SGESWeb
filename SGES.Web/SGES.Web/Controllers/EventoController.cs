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
            evento.IdAdmin = UsuarioActual.Id;

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
        public ActionResult InicioAprendiz(EventoFiltroModel filtro)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            var eventos = _dao.ObtenerEventos() ?? new List<EventoModel>();

            // Filtros opcionales — si el campo está vacío, no filtra
            if (!string.IsNullOrWhiteSpace(filtro.Nombre))
                eventos = eventos.Where(e =>
                    e.NombreEvento.IndexOf(filtro.Nombre,
                    StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            if (filtro.Fecha.HasValue)
                eventos = eventos.Where(e =>
                    e.FechaHoraInicio.Date == filtro.Fecha.Value.Date).ToList();

            if (!string.IsNullOrWhiteSpace(filtro.Modalidad))
                eventos = eventos.Where(e =>
                    e.ModalidadEvento == filtro.Modalidad).ToList();

            if (!string.IsNullOrWhiteSpace(filtro.TipoInscrip))
                eventos = eventos.Where(e =>
                    e.TipoInscrip == filtro.TipoInscrip).ToList();

            // Siempre al final: solo eventos futuros y ordenados
            var disponibles = eventos
                .Where(e => e.FechaHoraInicio >= DateTime.Now)
                .OrderBy(e => e.FechaHoraInicio)
                .ToList();

            ViewBag.Filtro = filtro;
            ViewBag.Modalidades = ObtenerModalidades();
            ViewBag.TiposInscripcion = ObtenerTiposInscripcion();

            return View(disponibles);

        }

        [HttpGet]
        public ActionResult InicioAdmin(EventoFiltroModel filtro)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            var eventos = _dao.ObtenerEventos() ?? new List<EventoModel>();

            if (!string.IsNullOrWhiteSpace(filtro.Nombre))
                eventos = eventos.Where(e =>
                    e.NombreEvento.IndexOf(filtro.Nombre,
                    StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            
            if (filtro.Fecha.HasValue)
                eventos = eventos.Where(e =>
                e.FechaHoraInicio.Date == filtro.Fecha.Value.Date).ToList();

            if (!string.IsNullOrWhiteSpace(filtro.Modalidad))
                eventos = eventos.Where(e =>
                   e.ModalidadEvento == filtro.Modalidad).ToList();

            if (!string.IsNullOrWhiteSpace(filtro.TipoInscrip))
                eventos = eventos.Where(e =>
                     e.TipoInscrip == filtro.TipoInscrip).ToList();

            var disponibles = eventos
                .Where(e => e.FechaHoraInicio >= DateTime.Now)
                .OrderBy(e => e.FechaHoraInicio)
                .ToList();

            ViewBag.Filtro = filtro;
            ViewBag.Modalidades = ObtenerModalidades();
            ViewBag.TiposInscripcion = ObtenerTiposInscripcion();

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
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");


            var evento = _dao.ObtenerEventos()
                .FirstOrDefault(e => e.IdEvento == id);


            if (evento == null)
            {
                TempData["Error"] = "Evento no encontrado.";
                return RedirectToAction("InicioAprendiz");
            }


            if (_inscripcionDao.YaInscrito(UsuarioActual.Id, id))
            {
                TempData["Error"] =
                    "Ya estás inscrito en este evento.";

                return RedirectToAction("InicioAprendiz");
            }


            ViewBag.Evento = evento;


            var inscripcion = new InscripcionModel
            {
                IdEvento = id,
                IdApr = UsuarioActual.Id
            };


            return View(inscripcion);
        }

        // ───────────────────────────────────────────────────────────────────
        // POST: /Evento/Details
        // Procesa la inscripción del aprendiz.
        // ────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(InscripcionModel inscripcion)
        {
            // Verificamos sesión
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");


            // Recuperamos el evento enviado desde el formulario
            var evento = _dao.ObtenerEventos()
                .FirstOrDefault(e => e.IdEvento == inscripcion.IdEvento);


            // Si no existe el evento
            if (evento == null)
            {
                TempData["Error"] = "Evento no encontrado.";
                return RedirectToAction("InicioAprendiz");
            }


            // El usuario de sesión es el aprendiz que se está inscribiendo
            inscripcion.IdApr = UsuarioActual.Id;


            // Fecha actual de inscripción
            inscripcion.FechaInscrip = DateTime.Now.Date;


            // Validar inscripción duplicada
            if (_inscripcionDao.YaInscrito(
                inscripcion.IdApr,
                inscripcion.IdEvento))
            {
                TempData["Error"] =
                    "Ya estás inscrito en este evento.";

                return RedirectToAction("InicioAprendiz");
            }


            // Validar cruces de horario
            if (_inscripcionDao.TieneCruceDeHorario(
                inscripcion.IdApr,
                inscripcion.IdEvento))
            {
                TempData["Error"] =
                    "Ya tienes otro evento en ese horario.";

                return RedirectToAction("InicioAprendiz");
            }


            // Validar cupos
            if (evento.CupoMaximo > 0)
            {
                int inscritos =
                    _inscripcionDao.ContarInscritos(
                        inscripcion.IdEvento);


                if (inscritos >= evento.CupoMaximo)
                {
                    TempData["Error"] =
                        "El evento ya no tiene cupos disponibles.";

                    return RedirectToAction("InicioAprendiz");
                }
            }


            try
            {
                // Inserta en tabla Inscripciones
                _inscripcionDao.Inscribir(inscripcion);


                TempData["Success"] =
                    "Te has inscrito correctamente.";

                return RedirectToAction("InicioAprendiz");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error al realizar inscripción: "
                    + ex.Message;

                return RedirectToAction("InicioAprendiz");
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
                ViewBag.IdEvento = idEvento.Value;
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
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            if (id == null)
            {
                TempData["Error"] = "Debe seleccionar un evento para modificar.";
                return RedirectToAction("InicioAdmin");
            }

            var evento = _dao.ObtenerEventos().FirstOrDefault(e => e.IdEvento == id.Value);

            if (evento == null)
            {
                TempData["Error"] = "El evento seleccionado no existe.";
                return RedirectToAction("InicioAdmin");
            }

            // ← Nueva validación
            int inscritos = _inscripcionDao.ContarInscritos(id.Value);
            if (inscritos > 0)
            {
                TempData["Error"] = $"No se puede modificar el evento '{evento.NombreEvento}' porque tiene {inscritos} aprendiz(ces) inscrito(s).";
                return RedirectToAction("InicioAdmin");
            }

            ViewBag.TiposEvento = ObtenerTiposEvento();
            ViewBag.Modalidades = ObtenerModalidades();
            ViewBag.TiposInscripcion = ObtenerTiposInscripcion();

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

        public ActionResult ReporteEventos()
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            var eventos = _dao.ObtenerEventos() ?? new List<EventoModel>();

            var reporte = new FastReport.Report();
            var rutaFrx = Server.MapPath("~/Reports/ReporteEventos.frx");
            reporte.Load(rutaFrx);

            // Registrar datos
            var tabla = new System.Data.DataTable("Eventos");
            tabla.Columns.Add("ID", typeof(int));
            tabla.Columns.Add("Nombre", typeof(string));
            tabla.Columns.Add("Tipo", typeof(string));
            tabla.Columns.Add("Modalidad", typeof(string));
            tabla.Columns.Add("TipoInscrip", typeof(string));
            tabla.Columns.Add("CupoMaximo", typeof(int));
            tabla.Columns.Add("Inicio", typeof(string));
            tabla.Columns.Add("Fin", typeof(string));

            foreach (var e in eventos)
            {
                tabla.Rows.Add(
                    e.IdEvento,
                    e.NombreEvento,
                    e.TipoEvento,
                    e.ModalidadEvento,
                    e.TipoInscrip,
                    e.CupoMaximo,
                    e.FechaHoraInicio.ToString("dd/MM/yyyy HH:mm"),
                    e.FechaHoraFin.ToString("dd/MM/yyyy HH:mm")
                );
            }

            reporte.RegisterData(tabla, "Eventos");
            reporte.Prepare();

            var export = new FastReport.Export.PdfSimple.PDFSimpleExport();
            using (var ms = new System.IO.MemoryStream())
            {
                export.Export(reporte, ms);
                return File(ms.ToArray(),
                    "application/pdf",
                    "ReporteEventos.pdf");
            }
        }

        public ActionResult ReporteAprendices(int idEvento)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            var aprendices = _dao.ObtenerAprendicesPorEvento(idEvento)
                            ?? new List<AprendizModel>();
            var evento = _dao.ObtenerEventos()
                            .FirstOrDefault(e => e.IdEvento == idEvento);

            var reporte = new FastReport.Report();
            var rutaFrx = Server.MapPath("~/Reports/ReporteAprendices.frx");
            reporte.Load(rutaFrx);

            var tabla = new System.Data.DataTable("Aprendices");
            tabla.Columns.Add("ID", typeof(int));
            tabla.Columns.Add("Nombre", typeof(string));
            tabla.Columns.Add("Email", typeof(string));
            tabla.Columns.Add("Contacto", typeof(string));

            foreach (var a in aprendices)
            {
                tabla.Rows.Add(
                    a.IdApr,
                    a.NombreApr,
                    a.EmailApr,
                    a.ContactoApr
                );
            }

            reporte.RegisterData(tabla, "Aprendices");
            reporte.Prepare();

            var nombreArchivo = evento != null
                ? $"Aprendices_{evento.NombreEvento}.xlsx"
                : "Aprendices.xlsx";

            var export = new FastReport.Export.PdfSimple.PDFSimpleExport();
            using (var ms = new System.IO.MemoryStream())
            {
                export.Export(reporte, ms);
                return File(ms.ToArray(),
                    "application/pdf",
                    nombreArchivo.Replace(".xlsx", ".pdf"));
            }
        }

        [HttpGet]
        public ActionResult InscribirGrupo(int id)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            var evento = _dao.ObtenerEventos().FirstOrDefault(e => e.IdEvento == id);
            if (evento == null || evento.TipoInscrip != "Grupal")
                return RedirectToAction("InicioAprendiz");

            if (_inscripcionDao.YaInscrito(UsuarioActual.Id, id))
            {
                TempData["Error"] = "Ya estás inscrito en este evento.";
                return RedirectToAction("InicioAprendiz");
            }

            var aprendizDao = new AprendizDAO();
            ViewBag.Evento = evento;
            ViewBag.Disponibles = aprendizDao.ObtenerAprendicesDisponibles(id, UsuarioActual.Id);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InscribirGrupo(int idEvento, List<int> seleccionados)
        {
            if (UsuarioActual == null)
                return RedirectToAction("Login", "Auth");

            var evento = _dao.ObtenerEventos().FirstOrDefault(e => e.IdEvento == idEvento);

            // El aprendiz actual siempre se incluye
            if (seleccionados == null) seleccionados = new List<int>();
            if (!seleccionados.Contains(UsuarioActual.Id))
                seleccionados.Insert(0, UsuarioActual.Id);

            // Validar límite de 3
            if (seleccionados.Count > 3)
            {
                TempData["Error"] = "El grupo no puede tener más de 3 integrantes.";
                return RedirectToAction("InscribirGrupo", new { id = idEvento });
            }

            // Validar que ninguno ya esté inscrito 
            foreach (int idApr in seleccionados)
            {
                if (_inscripcionDao.YaInscrito(idApr, idEvento))
                {
                    TempData["Error"] =
                        "Uno o más integrantes del grupo ya están inscritos en este evento.";
                    return RedirectToAction("InscribirGrupo", new { id = idEvento });
                }
            }

            // Validar que ninguno tenga cruce de horario
            foreach (int idApr in seleccionados)
            {
                if (_inscripcionDao.TieneCruceDeHorario(idApr, idEvento))
                {
                    TempData["Error"] = "Uno o más integrantes tienen cruce de horario.";
                    return RedirectToAction("InscribirGrupo", new { id = idEvento });
                }
            }

            // Validar cupo del evento
            if (evento.CupoMaximo > 0)
            {
                int inscritos = _inscripcionDao.ContarInscritos(idEvento);
                if (inscritos + seleccionados.Count > evento.CupoMaximo)
                {
                    TempData["Error"] = "No hay suficientes cupos para todo el grupo.";
                    return RedirectToAction("InscribirGrupo", new { id = idEvento });
                }
            }

            _inscripcionDao.InscribirGrupo(seleccionados, idEvento, DateTime.Today);
            TempData["Success"] = "Grupo inscrito correctamente.";
            return RedirectToAction("InicioAprendiz");
        }
    }
}