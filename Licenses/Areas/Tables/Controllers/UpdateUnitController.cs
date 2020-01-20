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
    public class UpdateUnitController : Controller
    {
        string CurrentUser { get; set; }

        // GET: Tables/UpdateUnit/idCount
        public ActionResult Index(long? id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            if (!id.HasValue)
                return Content("No Update Count record id supplied (idCount)");

            UpdateCountModel model = new UpdateCountModel();
            using (var access = DataAccess.Factory.GetTableAccess())
            {
                model = access.GetUpdateCountWithUnits(id.Value);
            }

            return View(model);
        }

        // GET: Tables/UpdateUnit/Details/5
        public ActionResult Details(long? id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            if (!id.HasValue)
                return Content("No Update Unit record id supplied (idUnit)");

            UpdateUnitModel model = new UpdateUnitModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id.Value, ref model))
                {
                    model.AvailableUnits = access.GetAvailableUnits();
                    return View(model);
                }
            }

            return View(model);
        }

        // GET: Tables/UpdateUnit/Create
        public ActionResult Create(long? id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            if (!id.HasValue)
                return Content("No Update Count record id supplied (id)");

            UpdateUnitModel model = new UpdateUnitModel();
            using (var access = Factory.GetTableAccess())
            {
                UpdateCountModel count = access.GetUpdateCount(id.Value);
                if (count == null)
                    return Content($"No Update Count Record exists with the id {id.Value}");

                model.updunit_upd_id = id.Value;
                model.UpdateDate = count.UpdateDate;
                model.ClientName = count.ClientName;
                model.ProductName = count.ProductName;
                model.AvailableUnits = access.GetAvailableUnits();
            }

            return View(model);
        }

        // POST: Tables/UpdateUnit/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add insert logic here
                UpdateUnitModel update = new UpdateUnitModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    UpdateCountModel count = access.GetUpdateCount(update.updunit_upd_id);
                    if (count == null)
                        return Content($"No Update Count Record exists with the id {update.updunit_upd_id}");

                    TryValidateModel(update);
                    if (!ModelState.IsValid)
                    {
                        update.UpdateDate = count.UpdateDate;
                        update.ClientName = count.ClientName;
                        update.ProductName = count.ProductName;
                        update.AvailableUnits = access.GetAvailableUnits();
                        return View(update);
                    }

                    int cnt = access.AddUpdateUnit(update);
                }

                return RedirectToAction("Index", new { id = update.updunit_upd_id, auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/UpdateUnit/Edit/5
        public ActionResult Edit(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UpdateUnitModel update = new UpdateUnitModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id, ref update))
                {
                    update.AvailableUnits = access.GetAvailableUnits();
                    return View(update);
                }

                UpdateCountModel count = access.GetUpdateCount(update.updunit_upd_id);
                if (count == null)
                    return Content($"No Update Count Record exists with the id {update.updunit_upd_id}");

                update.UpdateDate = count.UpdateDate;
                update.ClientName = count.ClientName;
                update.ProductName = count.ProductName;
                update.AvailableUnits = access.GetAvailableUnits();
            }

            return View(update);
        }

        // POST: Tables/UpdateUnit/Edit/5
        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add update logic here
                UpdateUnitModel update = new UpdateUnitModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    if (!CheckExists(access, id))
                    {
                        update.AvailableUnits = access.GetAvailableUnits();
                        return View(update);
                    }

                    int cnt = access.UpdateUpdateUnit(id, collection);
                }

                return RedirectToAction("Index", new { id = update.updunit_upd_id, auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/UpdateUnit/Delete/5
        public ActionResult Delete(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UpdateUnitModel update = new UpdateUnitModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id, ref update))
                    return View(update);

                UpdateCountModel count = access.GetUpdateCount(update.updunit_upd_id);
                if (count == null)
                    return Content($"No Update Count Record exists with the id {update.updunit_upd_id}");

                update.UpdateDate = count.UpdateDate;
                update.ClientName = count.ClientName;
                update.ProductName = count.ProductName;
            }

            return View(update);
        }

        // POST: Tables/UpdateUnit/Delete/5
        [HttpPost]
        public ActionResult Delete(long id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add delete logic here
                UpdateUnitModel update = new UpdateUnitModel();
                using (var access = Factory.GetTableAccess())
                {
                    if (!CheckExists(access, id, ref update))
                        return View(update);
                    int cnt = access.DeleteUpdateUnit(id);
                }

                return RedirectToAction("Index", new { id = update.updunit_upd_id, auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        private bool CheckExists(ITableAccess access, long id)
        {
            UpdateUnitModel model = access.GetUpdateUnit(id);
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "No UpdateUnit with an Id of " + id + " was found.");
                return false;
            }

            return true;
        }

        private bool CheckExists(ITableAccess access, long id, ref UpdateUnitModel model)
        {
            model = access.GetUpdateUnit(id);
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "No UpdateUnit with an Id of " + id + " was found.");
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
