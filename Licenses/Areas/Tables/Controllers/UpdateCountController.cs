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
    public class UpdateCountController : Controller
    {
        string CurrentUser { get; set; }

        // GET: Tables/UpdateCount
        public ActionResult Index()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UpdateCountSearchModel model = new UpdateCountSearchModel { StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddHours(11).AddMinutes(59).AddSeconds(59) };
            using (var access = Factory.GetTableAccess())
            {
                model.AvailableClients = access.GetAvailableClients(true);
                model.AvailableProducts = access.GetAvailableProducts(true);
                model.Updates = access.GetUpdateCounts(model.StartDate, model.EndDate);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                UpdateCountSearchModel model = new UpdateCountSearchModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    model.AvailableClients = access.GetAvailableClients(true);
                    model.AvailableProducts = access.GetAvailableProducts(true);
                    model.Updates = access.GetUpdateCounts(model.StartDate, model.EndDate, model.ClientId, model.ProductId);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/UpdateCount/Details/5
        public ActionResult Details(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UpdateCountModel model = new UpdateCountModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id, ref model))
                    return View(model);
            }

            return View(model);
        }

        // GET: Tables/UpdateCount/Create
        public ActionResult Create()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UpdateCountModel model = new UpdateCountModel();
            using (var access = Factory.GetTableAccess())
            {
                model.AvailableClients = access.GetAvailableClients();
                model.AvailableProducts = access.GetAvailableProducts();
                model.UpdateDate = DateTime.Now;
            }

            return View(model);
        }

        // POST: Tables/UpdateCount/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add insert logic here
                UpdateCountModel update = new UpdateCountModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    TryValidateModel(update);
                    if (!ModelState.IsValid)
                    {
                        update.AvailableClients = access.GetAvailableClients();
                        update.AvailableProducts = access.GetAvailableProducts();
                        return View(update);
                    }

                    int cnt = access.AddUpdateCount(update);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/UpdateCount/Edit/5
        public ActionResult Edit(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UpdateCountModel update = new UpdateCountModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id, ref update))
                {
                    update.AvailableClients = access.GetAvailableClients();
                    update.AvailableProducts = access.GetAvailableProducts();
                    return View(update);
                }
            }

            return View(update);
        }

        // POST: Tables/UpdateCount/Edit/5
        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add product logic here
                UpdateCountModel update = new UpdateCountModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    if (!CheckExists(access, id))
                    {
                        update.AvailableClients = access.GetAvailableClients();
                        update.AvailableProducts = access.GetAvailableProducts();
                        return View(update);
                    }

                    int cnt = access.UpdateUpdateCount(id, collection);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/UpdateCount/Delete/5
        public ActionResult Delete(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            UpdateCountModel update = new UpdateCountModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id, ref update))
                    return View(update);
            }

            return View(update);
        }

        // POST: Tables/UpdateCount/Delete/5
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
                    int cnt = access.DeleteUpdateCount(id);
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
            UpdateCountModel model = access.GetUpdateCount(id);
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "No UpdateCount with an Id of " + id + " was found.");
                return false;
            }

            return true;
        }

        private bool CheckExists(ITableAccess access, long id, ref UpdateCountModel model)
        {
            model = access.GetUpdateCount(id);
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "No UpdateCount with an Id of " + id + " was found.");
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
