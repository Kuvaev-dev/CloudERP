using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CloudERP.Models;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class AccountControlController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly List<AccountControlMV> _accountControl;

        public AccountControlController(CloudDBEntities db, List<AccountControlMV> accountControl)
        {
            _db = db;
            _accountControl = accountControl;
        }

        // GET: AccountControl
        public ActionResult Index()
        {
            try
            {
                _accountControl.Clear();

                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                var tblAccountControl = _db.tblAccountControl
                    .Include(t => t.tblBranch)
                    .Include(t => t.tblCompany)
                    .Include(t => t.tblUser)
                    .Where(a => a.CompanyID == companyID && a.BranchID == branchID);

                foreach (var item in tblAccountControl)
                {
                    _accountControl.Add(new AccountControlMV
                    {
                        AccountControlID = item.AccountControlID,
                        AccountControlName = item.AccountControlName,
                        AccountHeadID = item.AccountHeadID,
                        AccountHeadName = _db.tblAccountHead.Find(item.AccountHeadID)?.AccountHeadName,
                        BranchID = item.BranchID,
                        BranchName = item.tblBranch?.BranchName,
                        CompanyID = item.CompanyID,
                        Name = item.tblCompany?.Name,
                        UserID = item.UserID,
                        UserName = item.tblUser?.UserName
                    });
                }

                return View(_accountControl.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: AccountControl/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName");
                return View(new tblAccountControl());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: AccountControl/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountControl tblAccountControl)
        {
            try
            {
                _accountControl.Clear();

                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                tblAccountControl.CompanyID = companyID;
                tblAccountControl.BranchID = branchID;
                tblAccountControl.UserID = userID;

                if (ModelState.IsValid)
                {
                    var findControl = _db.tblAccountControl.FirstOrDefault(a => a.CompanyID == companyID &&
                                                                                a.BranchID == branchID &&
                                                                                a.AccountControlName == tblAccountControl.AccountControlName);
                    if (findControl == null)
                    {
                        _db.tblAccountControl.Add(tblAccountControl);
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = Resources.Messages.AlreadyExists;
                    }
                }

                ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
                return View(tblAccountControl);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: AccountControl/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                tblAccountControl tblAccountControl = _db.tblAccountControl.Find(id);
                if (tblAccountControl == null)
                {
                    return HttpNotFound();
                }

                ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
                return View(tblAccountControl);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: AccountControl/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountControl tblAccountControl)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int userID = Convert.ToInt32(Session["UserID"]);
                tblAccountControl.UserID = userID;

                if (ModelState.IsValid)
                {
                    var findControl = _db.tblAccountControl.FirstOrDefault(a => a.CompanyID == tblAccountControl.CompanyID &&
                                                                                a.BranchID == tblAccountControl.BranchID &&
                                                                                a.AccountControlName == tblAccountControl.AccountControlName &&
                                                                                a.AccountControlID != tblAccountControl.AccountControlID);
                    if (findControl == null)
                    {
                        _db.Entry(tblAccountControl).State = EntityState.Modified;
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = Resources.Messages.AlreadyExists;
                    }
                }

                ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
                return View(tblAccountControl);
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