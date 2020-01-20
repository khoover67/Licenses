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
    public class ClientController : Controller
    {
        string CurrentUser { get; set; }

        // GET: Tables/Client
        public ActionResult Index()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            List<ClientModel> models = new List<ClientModel>();
            using (var access = Factory.GetTableAccess())
            {
                models = access.GetClients();
            }

            return View(models);
        }

        // GET: Tables/Client/Details/5
        public ActionResult Details(int id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            ClientModel model = new ClientModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(model);

                model = access.GetClient(id);
            }

            return View(model);
        }

        // GET: Tables/Client/Create
        public ActionResult Create()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            ClientModel model = new ClientModel();
            using (var access = Factory.GetTableAccess())
            {
                model.AvailableAccounts = access.GetAvailableAccounts();
            }

            return View(model);
        }

        // POST: Tables/Client/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add insert logic here
                ClientModel client = new ClientModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    TryValidateModel(client);
                    if (!ModelState.IsValid)
                    {
                        client.AvailableAccounts = access.GetAvailableAccounts();
                        return View(client);
                    }

                    if (access.GetClientId(client.cln_name, client.cln_db_path) > -1)
                    {
                        ModelState.AddModelError(string.Empty, "A client with the name '" + client.cln_name + "' and database path of '" + client.cln_db_path + "' already exists");
                        client.AvailableAccounts = access.GetAvailableAccounts();
                        return View(client);
                    }

                    int cnt = access.AddClient(client);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><div class='pdi-error'><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></div></html>");
            }
        }

        // GET: Tables/Client/Edit/5
        public ActionResult Edit(int id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            ClientModel client = new ClientModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(client);
                client = access.GetClient(id);
            }

            return View(client);
        }

        // POST: Tables/Client/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add update logic here
                ClientModel client = new ClientModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    if (!CheckExists(access, id))
                        return View(client);

                    TryValidateModel(client);
                    if (!ModelState.IsValid)
                    {
                        client.AvailableAccounts = access.GetAvailableAccounts();
                        return View(client);
                    }

                    long idCopy = access.GetClientId(client.cln_name, client.cln_db_path);
                    if (idCopy > -1 && idCopy != id)
                    {
                        ModelState.AddModelError(string.Empty, "A client with the name '" + client.cln_name + "' and database path of '" + client.cln_db_path + "' already exists");
                        return View(client);
                    }

                    int cnt = access.UpdateClient(id, collection);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/Client/Delete/5
        public ActionResult Delete(int id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            ClientModel client = new ClientModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(client);
                client = access.GetClient(id);
            }

            return View(client);
        }

        // POST: Tables/Client/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add delete logic here
                using (var access = Factory.GetTableAccess())
                {
                    int cnt = access.DeleteClient(id);
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
            var client = access.GetClient(id);
            if (client == null)
            {
                ModelState.AddModelError(string.Empty, "No Client with an Id of " + id + " was found.");
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
