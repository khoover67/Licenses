using Licenses.DataAccess;
using Licenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Controllers
{
    public class SettingsController : Controller
    {
        public string CurrentUser { get; set; }

        // GET: Settings
        public ActionResult Index()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login");

            using (var access = Factory.GetSettingAccess())
            {
                SettingListModel settings = new SettingListModel
                {
                    AuthToken = Auth.GetToken(CurrentUser),
                    Settings = access.GetSettings()
                };
                
                return View(settings);
            }
        }

        // GET: Settings/Details/5
        public ActionResult Details(string id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login");

            using (var access = Factory.GetSettingAccess())
            {
                SettingModel model = access.GetSetting(id);
                if (model != null)
                {
                    return View(model);
                }
            }

            return View();
        }

        // GET: Settings/Create
        public ActionResult Create()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login");

            SettingModel model = new SettingModel();
            return View(model);
        }

        // POST: Settings/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (!ValidateToken())
                    return RedirectToAction("Index", "Login");

                using (var access = Factory.GetSettingAccess())
                {
                    SettingModel model = new SettingModel(collection);
                    this.TryValidateModel(model);
                    if (!ModelState.IsValid)
                        return View(model);

                    SettingModel original = access.GetSetting(model.set_name);
                    if (original != null)
                    {
                        ModelState.AddModelError(string.Empty, $"The Setting '{model.set_name}' already exists.");
                        return View(original);
                    }

                    access.AddSetting(model);
                }

                return RedirectToAction("Index", new { auth = Auth.GetToken(CurrentUser) });
            }
            catch
            {
                return View();
            }
        }

        // GET: Settings/Edit/5
        public ActionResult Edit(string id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login");

            using (ISettingAccess access = Factory.GetSettingAccess())
            {
                if (!CheckExists(access, id))
                    return View(new SettingModel());

                SettingModel setting = access.GetSetting(id);
                return View(setting);
            }
        }

        // POST: Settings/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login");

            try
            {
                // TODO: Add update logic here
                using (DataAccess.ISettingAccess access = DataAccess.Factory.GetSettingAccess())
                {
                    SettingModel model = new SettingModel(collection);
                    if (!CheckExists(access, id))
                        return View(model);

                    this.TryValidateModel(model);
                    if (!ModelState.IsValid)
                        return View(model);

                    if (!CheckExists(access, model.set_name))
                    {
                        return View(model);
                    }

                    access.UpdateSetting(id, collection);
                }

                return RedirectToAction("Index", new { auth = Auth.GetToken(CurrentUser) });
            }
            catch
            {
                return View();
            }
        }

        // GET: Settings/Delete/5
        public ActionResult Delete(string id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login");

            using (ISettingAccess access = Factory.GetSettingAccess())
            {
                if (!CheckExists(access, id))
                    return View(new SettingModel());

                SettingModel Setting = access.GetSetting(id);
                return View(Setting);
            }
        }

        // POST: Settings/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login");

            try
            {
                // TODO: Add delete logic here
                using (ISettingAccess access = Factory.GetSettingAccess())
                {
                    if (!CheckExists(access, id))
                        return View(new SettingModel());

                    access.DeleteSetting(id);
                }

                return RedirectToAction("Index", new { auth = Auth.GetToken(CurrentUser) });
            }
            catch
            {
                return View();
            }
        }

        private bool CheckExists(DataAccess.ISettingAccess access, string id)
        {
            var client = access.GetSetting(id);
            if (client == null)
            {
                ModelState.AddModelError(string.Empty, "No Setting with Name of " + id + " was found.");
                return false;
            }

            return true;
        }

        bool ValidateToken()
        {
            if (Request == null || Request.Params == null || Request.Params.AllKeys.Length == 0)
                return false;

            CurrentUser = Auth.IsValidToken(Request["auth"]);
            return (!string.IsNullOrWhiteSpace(CurrentUser));
        }
    }
}
