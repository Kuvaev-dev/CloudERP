using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class AccountSettingController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SessionHelper _sessionHelper;

        public AccountSettingController(CloudDBEntities db, SessionHelper sessionHelper)
        {
            _db = db;
            _sessionHelper = sessionHelper;
        }

        // GET: AccountSetting
        public ActionResult Index()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var tblAccountSetting = _db.tblAccountSetting
                    .Include(t => t.tblAccountActivity)
                    .Include(t => t.tblAccountControl)
                    .Include(t => t.tblAccountHead)
                    .Include(t => t.tblBranch)
                    .Include(t => t.tblCompany)
                    .Where(t => t.CompanyID == _sessionHelper.CompanyID && t.BranchID == _sessionHelper.BranchID);

                return View(tblAccountSetting.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: AccountSetting/Create
        public ActionResult Create()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                ViewBag.AccountActivityID = new SelectList(_db.tblAccountActivity, "AccountActivityID", "Name", "0");
                ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(c => c.BranchID == _sessionHelper.BranchID && c.CompanyID == _sessionHelper.CompanyID), "AccountControlID", "AccountControlName", "0");
                ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", "0");
                ViewBag.AccountSubControlID = new SelectList(_db.tblAccountSubControl.Where(c => c.BranchID == _sessionHelper.BranchID && c.CompanyID == _sessionHelper.CompanyID), "AccountSubControlID", "AccountSubControlName", "0");

                return View(new tblAccountSetting());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: AccountSetting/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountSetting tblAccountSetting)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                tblAccountSetting.BranchID = _sessionHelper.BranchID;
                tblAccountSetting.CompanyID = _sessionHelper.CompanyID;

                if (ModelState.IsValid)
                {
                    var findSetting = _db.tblAccountSetting
                        .FirstOrDefault(c => c.CompanyID == tblAccountSetting.CompanyID &&
                                             c.BranchID == tblAccountSetting.BranchID &&
                                             c.AccountActivityID == tblAccountSetting.AccountActivityID);

                    if (findSetting == null)
                    {
                        _db.tblAccountSetting.Add(tblAccountSetting);
                        _db.SaveChanges();
                        ViewBag.Message = Resources.Messages.SavedSuccessfully;

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = Resources.Messages.AlreadyExists;
                    }
                }

                ViewBag.AccountActivityID = new SelectList(_db.tblAccountActivity, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
                ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(c => c.BranchID == _sessionHelper.BranchID && c.CompanyID == _sessionHelper.CompanyID), "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
                ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
                ViewBag.AccountSubControlID = new SelectList(_db.tblAccountSubControl.Where(c => c.BranchID == _sessionHelper.BranchID && c.CompanyID == _sessionHelper.CompanyID), "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);

                return View(tblAccountSetting);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: AccountSetting/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                tblAccountSetting tblAccountSetting = _db.tblAccountSetting.Find(id);
                if (tblAccountSetting == null)
                {
                    return HttpNotFound();
                }

                ViewBag.AccountActivityID = new SelectList(_db.tblAccountActivity, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
                ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(c => c.BranchID == _sessionHelper.BranchID && c.CompanyID == _sessionHelper.CompanyID), "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
                ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
                ViewBag.AccountSubControlID = new SelectList(_db.tblAccountSubControl.Where(c => c.BranchID == _sessionHelper.BranchID && c.CompanyID == _sessionHelper.CompanyID), "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);

                return View(tblAccountSetting);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: AccountSetting/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountSetting tblAccountSetting)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                if (ModelState.IsValid)
                {
                    var findSetting = _db.tblAccountSetting
                        .FirstOrDefault(c => c.CompanyID == tblAccountSetting.CompanyID &&
                                             c.BranchID == tblAccountSetting.BranchID &&
                                             c.AccountActivityID == tblAccountSetting.AccountActivityID &&
                                             c.AccountSettingID != tblAccountSetting.AccountSettingID);

                    if (findSetting == null)
                    {
                        _db.Entry(tblAccountSetting).State = EntityState.Modified;
                        _db.SaveChanges();
                        ViewBag.Message = Resources.Messages.UpdatedSuccessfully;

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = Resources.Messages.AlreadyExists;
                    }
                }

                ViewBag.AccountActivityID = new SelectList(_db.tblAccountActivity, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
                ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(c => c.BranchID == _sessionHelper.BranchID && c.CompanyID == _sessionHelper.CompanyID), "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
                ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
                ViewBag.AccountSubControlID = new SelectList(_db.tblAccountSubControl.Where(c => c.BranchID == _sessionHelper.BranchID && c.CompanyID == _sessionHelper.CompanyID), "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);

                return View(tblAccountSetting);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpGet]
        public ActionResult GetAccountControls(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                List<AccountControlMV> controls = new List<AccountControlMV>();
                var controlList = _db.tblAccountControl
                    .Where(p => p.BranchID == _sessionHelper.BranchID && p.CompanyID == _sessionHelper.CompanyID && p.AccountHeadID == id)
                    .ToList();

                foreach (var item in controlList)
                {
                    controls.Add(new AccountControlMV() { AccountControlID = item.AccountControlID, AccountControlName = item.AccountControlName });
                }

                return Json(new { data = controls }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpGet]
        public ActionResult GetSubControls(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                List<AccountSubControlMV> subControls = new List<AccountSubControlMV>();
                var subControlList = _db.tblAccountSubControl
                    .Where(p => p.BranchID == _sessionHelper.BranchID && p.CompanyID == _sessionHelper.CompanyID && p.AccountControlID == id)
                    .ToList();

                foreach (var item in subControlList)
                {
                    subControls.Add(new AccountSubControlMV() { AccountSubControlID = item.AccountSubControlID, AccountSubControlName = item.AccountSubControlName });
                }

                return Json(new { data = subControls }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}