using SGESWeb.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SGESWeb.Controllers
{
    public class EventoController : Controller
    {
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
        public ActionResult CrearEvento(EventoModel evento)
        {
            if (evento.FechaHoraInicio < DateTime.Now)
            {
                ModelState.AddModelError(
                    "FechaHoraInicio",
                    "La fecha de inicio no puede ser anterior a la fecha y hora actual."
                );
            }

            if (evento.FechaHoraFin != default && evento.FechaHoraInicio != default
                && evento.FechaHoraFin < evento.FechaHoraInicio)
            {
                ModelState.AddModelError(
                    "FechaHoraFin",
                    "La fecha de fin debe ser posterior a la fecha de inicio."
                );
            }

            if (ModelState.IsValid)
            {
                EventoDAO dao = new EventoDAO();
                dao.InsertarEvento(evento);
                return RedirectToAction("CrearEvento");
            }

            ViewBag.TiposEvento = ObtenerTiposEvento();
            return View(evento);
        }
    }
}