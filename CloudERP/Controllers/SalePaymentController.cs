using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SalePaymentController : Controller
    {
        // GET: SalePayment
        public ActionResult RemainingPaymentList()
        {
            return View();
        }
    }
}