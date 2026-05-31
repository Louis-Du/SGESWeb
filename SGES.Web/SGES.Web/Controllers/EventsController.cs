using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGES.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public ActionResult Index()
        {
            var eventos = _eventService.GetAvailableEvents();

            return View(eventos);
        }
    }
}