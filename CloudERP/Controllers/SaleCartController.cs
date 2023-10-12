using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SaleCartController : Controller
    {
        // GET: SaleCart
        public ActionResult NewSale()
        {
            return View();
        }
    }
}