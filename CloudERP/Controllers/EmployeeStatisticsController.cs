using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class EmployeeStatisticsController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SessionHelper _sessionHelper;

        public EmployeeStatisticsController(CloudDBEntities db, SessionHelper sessionHelper)
        {
            _db = db;
            _sessionHelper = sessionHelper;
        }

        // GET: EmployeeStatistics
        public ActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                DateTime start = startDate ?? DateTime.Now.AddMonths(-1);
                DateTime end = endDate ?? DateTime.Now;

                var statistics = GetEmployeeStatistics(start, end, _sessionHelper.BranchID, _sessionHelper.CompanyID);
                return View(statistics);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private List<EmployeeStatisticsMV> GetEmployeeStatistics(DateTime startDate, DateTime endDate, int branchID, int companyID)
        {
            try
            {
                var branchIDs = _db.tblBranch
                .Where(b => b.BrchID == branchID || b.BranchID == branchID)
                .Select(b => b.BrchID)
                .ToList();

                branchIDs.Add(branchID);

                var employeeData = _db.tblEmployee
                    .Where(e => e.RegistrationDate.HasValue
                           && e.RegistrationDate.Value >= startDate
                           && e.RegistrationDate.Value <= endDate
                           && e.CompanyID == companyID
                           && branchIDs.Contains(e.BranchID))
                    .ToList();

                var statistics = employeeData
                    .GroupBy(e => e.RegistrationDate.Value.Date)
                    .Select(g => new EmployeeStatisticsMV
                    {
                        Date = g.Key,
                        NumberOfRegistrations = g.Count()
                    })
                    .ToList();

                return statistics;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return null;
            }
        }
    }
}