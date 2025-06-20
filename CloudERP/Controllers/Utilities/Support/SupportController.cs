﻿using Domain.Models;
using Domain.UtilsAccess;
using Localization.CloudERP.Messages;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Utilities.Support
{
    public class SupportController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public SupportController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        public async Task<ActionResult> Support()
        {
            try
            {
                ViewBag.UserTickets = await _httpClient.GetAsync<IEnumerable<SupportTicket>>(
                    $"supportapi/getusertickets?userId={_sessionHelper.UserID}");

                return View(new SupportTicket());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitTicket(SupportTicket model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid) return View(model);

            try
            {
                var user = await _httpClient.GetAsync<Domain.Models.User>($"userapi/getbyid?id={_sessionHelper.UserID}");
                var userTickets = await _httpClient.GetAsync<IEnumerable<SupportTicket>>($"supportapi/getusertickets?userId={_sessionHelper.UserID}");

                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;
                model.DateCreated = DateTime.Now;
                model.IsResolved = false;
                model.Email = user?.Email;
                model.Name = $"{model.DateCreated} - {user?.FullName}";
                model.AdminResponse = string.Empty;
                model.RespondedBy = string.Empty;
                model.ResponseDate = null;

                var success = await _httpClient.PostAsync("supportapi/submitticket", model);
                if (success) ViewBag.UserTickets = userTickets;
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AdminList()
        {
            try
            {
                var tickets = await _httpClient.GetAsync<IEnumerable<SupportTicket>>("supportapi/getadminlist");
                if (tickets == null) return RedirectToAction("AdminList");

                return View(tickets);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResolveTicket(int id, string responseMessage)
        {
            try
            {
                var ticket = await _httpClient.GetAsync<SupportTicket>($"supportapi/getbyid?id={id}");
                var admin = await _httpClient.GetAsync<Domain.Models.User>($"userapi/getbyid?id={_sessionHelper.UserID}");

                if (ticket == null) return RedirectToAction("AdminList");

                ticket.AdminResponse = responseMessage;
                ticket.RespondedBy = admin?.FullName;
                ticket.ResponseDate = DateTime.Now;
                ticket.IsResolved = true;

                var success = await _httpClient.PostAsync($"supportapi/resolveticket?id={id}", ticket);

                return RedirectToAction("AdminList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}