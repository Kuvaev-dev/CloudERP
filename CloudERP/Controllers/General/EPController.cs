using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.General
{
    public class EPController : Controller
    {
        // GET: EP404
        public ActionResult EP404() => View();

        // GET: EP500
        public ActionResult EP500() => View();

        // GET: EP600
        public ActionResult EP600() => View();
    }
}