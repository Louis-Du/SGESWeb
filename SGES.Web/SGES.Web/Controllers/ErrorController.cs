using System.Web.Mvc;

namespace SGES.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /Error/NotFound  (404)
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            ViewBag.StatusCode  = 404;
            ViewBag.RequestUrl  = Request.Url?.AbsolutePath;
            return View("Error");
        }

        // GET: /Error/Forbidden  (403)
        public ActionResult Forbidden()
        {
            Response.StatusCode = 403;
            ViewBag.StatusCode  = 403;
            ViewBag.RequestUrl  = Request.Url?.AbsolutePath;
            return View("Error");
        }

        // GET: /Error/ServerError  (500)
        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            ViewBag.StatusCode  = 500;
            ViewBag.RequestUrl  = Request.Url?.AbsolutePath;
            return View("Error");
        }

        // GET: /Error/General  (fallback)
        public ActionResult General()
        {
            ViewBag.StatusCode = 0;
            return View("Error");
        }
    }
}
