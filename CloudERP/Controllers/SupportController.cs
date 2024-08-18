using DatabaseAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SupportController : Controller
    {
        private readonly CloudDBEntities _db;

        public SupportController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: Support
        public ActionResult Support()
        {
            return View(new tblSupportTicket());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitTicket(tblSupportTicket ticket)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                if (ModelState.IsValid)
                {
                    ticket.Name = Convert.ToString(Session["EName"]);
                    ticket.Email = Convert.ToString(Session["Email"]);
                    ticket.DateCreated = DateTime.Now;
                    ticket.IsResolved = false;
                    ticket.CompanyID = companyID;
                    ticket.BranchID = branchID;
                    ticket.UserID = userID;

                    _db.tblSupportTicket.Add(ticket);
                    await _db.SaveChangesAsync();

                    ViewBag.Message = Resources.Messages.SupportRequestSubmitted;
                    return View("Support", new tblSupportTicket());
                }

                return View("Support", ticket);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult AdminList()
        {
            try
            {
                var tickets = _db.tblSupportTicket.ToList();
                return View(tickets);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ResolveTicket(int id)
        {
            try
            {
                var ticket = await _db.tblSupportTicket.FindAsync(id);
                if (ticket != null)
                {
                    ticket.IsResolved = true;
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("AdminList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}