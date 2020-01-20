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
    public class UnitController : Controller
    {
        string CurrentUser { get; set; }

        // GET: Tables/Unit
        public ActionResult Index()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            List<UnitModel> models = new List<UnitModel>();
            using (var access = Factory.GetTableAccess())
            {
                models = access.GetUnits();
            }

            return View(models);
        }

        // GET: Tables/Unit/Details/5
        public ActionResult Details(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UnitModel model = new UnitModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(model);

                model = access.GetUnit(id);
            }

            return View(model);
        }

        // GET: Tables/Unit/Create
        public ActionResult Create()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UnitModel model = new UnitModel();
            return View(model);
        }

        // POST: Tables/Unit/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add insert logic here
                UnitModel unit = new UnitModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    if (access.GetUnitId(unit.unit_name) > -1)
                    {
                        ModelState.AddModelError(string.Empty, "A Unit with the name '" + unit.unit_name + "' already exists");
                        return View(unit);
                    }

                    TryValidateModel(unit);
                    if (!ModelState.IsValid)
                        return View(unit);

                    int cnt = access.AddUnit(unit);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/Unit/Edit/5
        public ActionResult Edit(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UnitModel account = new UnitModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(account);
                account = access.GetUnit(id);
            }

            return View(account);
        }

        // POST: Tables/Unit/Edit/5
        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add update logic here
                UnitModel unit = new UnitModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    if (!CheckExists(access, id))
                        return View(unit);

                    TryValidateModel(unit);
                    if (!ModelState.IsValid)
                        return View(unit);

                    long idCopy = access.GetUnitId(unit.unit_name);
                    if (idCopy > -1 && idCopy != id)
                    {
                        ModelState.AddModelError(string.Empty, "A Unit with the name '" + (unit.unit_name ?? "") + "' already exists");
                        return View(unit);
                    }

                    int cnt = access.UpdateUnit(id, collection);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/Unit/Delete/5
        public ActionResult Delete(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UnitModel account = new UnitModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(account);
                account = access.GetUnit(id);
            }

            return View(account);
        }

        // POST: Tables/Unit/Delete/5
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
                    int cnt = access.DeleteUnit(id);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        private bool CheckExists(ITableAccess access, long id)
        {
            var account = access.GetUnit(id);
            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "No Unit with an Id of " + id + " was found.");
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
