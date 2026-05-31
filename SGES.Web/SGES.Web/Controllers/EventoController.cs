using SGESWeb.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;

namespace SGESWeb.Controllers
{
    public class EventoController : Controller
    {
        [HttpGet]
        public ActionResult CrearEvento()
        {
            ViewBag.TiposEvento = new SelectList(new List<string>
            {
                "Educativo",
                "Deportivo",
                "Social",
                "Cultural"
            });

            return View(new EventoModel());
        }

        [HttpPost]
        public ActionResult CrearEvento(EventoModel evento)
        {
            if (ModelState.IsValid)
            {
                EventoDAO dao = new EventoDAO();
                dao.InsertarEvento(evento);

                return RedirectToAction("CrearEvento");
            }

            // Se vuelve a enviar los valores del combobox en caso de que ocurra un error
            ViewBag.TiposEvento = new SelectList(new List<string>
            {
                "Educativo",
                "Deportivo",
                "Social",
                "Cultural"
            });

            return View(evento);
        }
    }
}