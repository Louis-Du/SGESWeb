using SGES.Web.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SGESWeb.Controllers
{
    public class EventoController : Controller
    {
        private readonly EventoDAO _dao;

        public EventoController() : this(new EventoDAO()) { }

        public EventoController(EventoDAO dao)
        {
            _dao = dao ?? throw new ArgumentNullException(nameof(dao));
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
            ViewBag.TiposEvento = ObtenerTiposEvento();
            return View(new EventoModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEvento(EventoModel evento)
        {
            ViewBag.TiposEvento = ObtenerTiposEvento();

            if (evento.FechaHoraInicio < DateTime.Now)
            {
                ModelState.AddModelError(
                    "FechaHoraInicio",
                    "La fecha de inicio no puede ser anterior a la fecha y hora actual."
                );
            }

            if (evento.FechaHoraFin != default(DateTime)
                && evento.FechaHoraInicio != default(DateTime)
                && evento.FechaHoraFin < evento.FechaHoraInicio)
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
                ModelState.AddModelError(
                    string.Empty,
                    "Error al guardar el evento: " + ex.Message
                );

                return View(evento);
            }
        }

        // NUEVA ACCIÓN: Listado de eventos disponibles para el aprendiz autenticado
        // URL: /Evento/Listado
        [HttpGet]
        // [Authorize] // HABILITAR HASTA QUE JAVIER HAGA EL LOGIN XD
        public ActionResult Listado()
        {
            var eventos = _dao.ObtenerEventos() ?? new List<EventoModel>();

            var disponibles = eventos
                .Where(e => e.FechaHoraInicio >= DateTime.Now)
                .OrderBy(e => e.FechaHoraInicio)
                .ToList();

            return View(disponibles);
        }
    }
}