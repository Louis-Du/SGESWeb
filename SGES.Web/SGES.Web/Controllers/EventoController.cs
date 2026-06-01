using System;
using System.Linq;
using System.Web.Mvc;
using SGES.Web.Models;
using System.Collections.Generic;

namespace SGES.Web.Controllers
{
    public class EventoController : Controller
    {
        private readonly EventoDAO _dao;

        public EventoController() : this(new EventoDAO()) { }

        public EventoController(EventoDAO dao)
        {
            _dao = dao ?? throw new ArgumentNullException(nameof(dao));
        }

        [HttpGet]
        public ActionResult CrearEvento()
        {
            PopulateTiposEvento();
            return View(new EventoModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEvento(EventoModel evento)
        {
            PopulateTiposEvento();

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

        private void PopulateTiposEvento()
        {
            ViewBag.TiposEvento = new SelectList(new List<string>
            {
                "Educativo",
                "Deportivo",
                "Social",
                "Cultural"
            });
        }

        // NUEVA ACCIÓN: Listado de eventos disponibles para el aprendiz autenticado
        // URL: /Evento/Listado
        [HttpGet]
        // [Authorize] // HABILITAR HASTA QUE JAVIER HAGA EL LOGIN XD
        public ActionResult Listado()
        {
            // Obtener todos los eventos desde el DAO
            var eventos = _dao.ObtenerEventos() ?? new List<EventoModel>();

            // Filtrar: sólo eventos futuros (disponibles)
            var disponibles = eventos
                .Where(e => e.FechaHoraInicio >= DateTime.Now)
                .OrderBy(e => e.FechaHoraInicio)
                .ToList();

            return View(disponibles); // Views/Evento/Listado.cshtml
        }
    }
}