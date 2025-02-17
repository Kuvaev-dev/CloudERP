using CloudERP.Helpers;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SupportController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public SupportController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> Support()
        {
            try
            {
                ViewBag.UserTickets = await _httpClient.GetAsync<List<SupportTicket>>($"support/{_sessionHelper.UserID}");

                return View(new SupportTicket());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла ошибка: " + ex.Message;
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitTicket(SupportTicket model)
        {
            try
            {
                var user = await _httpClient.GetAsync<User>($"user/{_sessionHelper.UserID}");
                var userTickets = await _httpClient.GetAsync<List<SupportTicket>>($"support/user/{_sessionHelper.UserID}");

                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;
                model.DateCreated = DateTime.Now;
                model.IsResolved = false;
                model.Email = user.Email;
                model.Name = $"{model.DateCreated} - {user.FullName}";
                model.AdminResponse = string.Empty;
                model.RespondedBy = string.Empty;
                model.ResponseDate = null;

                if (ModelState.IsValid)
                {
                    await _httpClient.PostAsync("support/create", model);
                    ViewBag.Message = Localization.CloudERP.Messages.Messages.SupportRequestSubmitted;

                    ViewBag.UserTickets = userTickets;

                    return View("Support", new SupportTicket());
                }

                ViewBag.UserTickets = userTickets;

                return View("Support", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AdminList()
        {
            try
            {
                var tickets = await _httpClient.GetAsync<List<SupportTicket>>("support/admin/list");
                if (tickets == null) return RedirectToAction("AdminList");

                return View(tickets);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResolveTicket(int id, string responseMessage)
        {
            try
            {
                var ticket = await _httpClient.GetAsync<SupportTicket>($"support/{id}");
                var admin = await _httpClient.GetAsync<User>($"user/{_sessionHelper.UserID}");

                if (ticket == null) return RedirectToAction("AdminList");

                ticket.AdminResponse = responseMessage;
                ticket.RespondedBy = admin.FullName;
                ticket.ResponseDate = DateTime.Now;
                ticket.IsResolved = true;

                var success = await _httpClient.PostAsync($"support/resolve/{id}", ticket);
                if (success) return RedirectToAction("Index");
                await _httpClient.PutAsync($"support/update/{ticket.TicketID}", ticket);

                return RedirectToAction("AdminList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}