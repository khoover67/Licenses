using Licenses.Areas.Tables.DataAccess;
using Licenses.Areas.Tables.Models;
using Licenses.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Controllers
{
    public class AccountController : Controller
    {
        string CurrentUser { get; set; }

        // GET: Tables/Account
        public ActionResult Index()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            List<AccountModel> models = new List<AccountModel>();
            using (var access = Factory.GetTableAccess())
            {
                models = access.GetAccounts();
            }

            return View(models);
        }

        // GET: Tables/Account/Details/5
        public ActionResult Details(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            AccountModel model = new AccountModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(model);

                model = access.GetAccount(id);
            }

            return View(model);
        }

        // GET: Tables/Account/Create
        public ActionResult Create()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            AccountModel model = new AccountModel();
            return View(model);
        }

        // POST: Tables/Account/Create
        [HttpPost]
        public ActionResult Create(AccountModel account)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add insert logic here
                //AccountModel account = new AccountModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    TryValidateModel(account);
                    if (!ModelState.IsValid)
                        return View(account);

                    if (access.GetAccountId(account.account_number) > -1)
                    {
                        ModelState.AddModelError(string.Empty, "An account with the number '" + account.account_number + "' already exists");
                        return View(account);
                    }

                    if (access.GetAccountId(account.account_name) > -1)
                    {
                        ModelState.AddModelError(string.Empty, "An account with the name '" + (account.account_name ?? "") + "' already exists");
                        return View(account);
                    }

                    int cnt = access.AddAccount(account);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre class='pdi-error'>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/Account/Edit/5
        public ActionResult Edit(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            AccountModel account = new AccountModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(account);
                account = access.GetAccount(id);
            }

            return View(account);
        }

        // POST: Tables/Account/Edit/5
        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add update logic here
                AccountModel account = new AccountModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    if (!CheckExists(access, id))
                        return View(account);

                    TryValidateModel(account);
                    if (!ModelState.IsValid)
                        return View(account);

                    long idCopy = access.GetAccountId(account.account_name);
                    if (idCopy > -1 && idCopy != id)
                    {
                        ModelState.AddModelError(string.Empty, "An account with the name '" + (account.account_name ?? "") + "' already exists");
                        return View(account);
                    }

                    idCopy = access.GetAccountId(account.account_number);
                    if (idCopy > -1 && idCopy != id)
                    {
                        ModelState.AddModelError(string.Empty, "An account with the number '" + account.account_number + "' already exists");
                        return View(account);
                    }

                    int cnt = access.UpdateAccount(id, collection);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch(Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre class='pdi-error'>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/Account/Delete/5
        public ActionResult Delete(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            AccountModel account = new AccountModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(account);
                account = access.GetAccount(id);
            }

            return View(account);
        }

        // POST: Tables/Account/Delete/5
        [HttpPost]
        public ActionResult Delete(long id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add delete logic here
                using (var access = Factory.GetTableAccess())
                {
                    int cnt = access.DeleteAccount(id);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre class='pdi-error'>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        private bool CheckExists(ITableAccess access, long id)
        {
            var account = access.GetAccount(id);
            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "No Account with an Id of " + id + " was found.");
                return false;
            }

            return true;
        }

        bool ValidateToken()
        {
            if (Request == null || Request.Params == null || Request.Params.AllKeys.Length == 0)
                return false;

            CurrentUser = Licenses.DataAccess.Auth.IsValidToken(Request["auth"]);
            ViewBag.AuthCode = Licenses.DataAccess.Auth.GetToken(CurrentUser);
            return (!string.IsNullOrWhiteSpace(CurrentUser));
        }
    }
}
