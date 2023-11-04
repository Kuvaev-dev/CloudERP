using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SalePaymentReturnController : Controller
    {
        // GET: SalePaymentReturn
        public ActionResult ReturnSalePendingAmount(int? id)
        {
            return View();
        }
    }
}