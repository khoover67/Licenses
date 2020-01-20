using Licenses.Areas.Updates.DataAccess;
using Licenses.Areas.Updates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Updates.Controllers
{
    public class HomeController : Controller
    {
        string CurrentUser { get; set; }

        // GET: Updates/Home
        public ActionResult Index(int idClient = -1, int idProduct = -1)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            return RedirectToAction("SearchMethod", new { auth = ViewBag.AuthCode });
        }

        // GET: Updates/SearchMethod
        public ActionResult SearchMethod()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            return View();
        }

        // GET: Updates/Home/Latest
        public ActionResult Latest(int id = -1)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            List<ClientModel> result = new List<ClientModel>();
            using (var access = Factory.GetUpdateAccess())
            {
                if (id > -1)
                {
                    ClientModel client = access.GetClient(id);
                    if (client != null)
                        result.Add(client);
                }
                else
                {
                    result = access.GetLatestUpdatesByClient();
                }
            }

            return View(result);
        }

        // GET: Updates/Home/SearchClientProduct
        public ActionResult SearchClientProduct()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            var model = new SearchClientProduct();
            using (var access = Factory.GetUpdateAccess())
            {
                var data = access.GetAllClientsAndProducts();
                if (data.Clients.Count < 1)
                {
                    ModelState.AddModelError(string.Empty, "No Clients were found.");
                    return View(model);
                }

                if (data.Products.Count < 1)
                {
                    ModelState.AddModelError(string.Empty, "No Products were found.");
                    return View(model);
                }

                foreach(var cln in data.Clients)
                {
                    model.AvailableClients.Add(new SelectListItem { Text = cln.cln_name + " (" + cln.cln_db_path + ")", Value = cln.cln_id.ToString() });
                }

                foreach (var prod in data.Products)
                {
                    model.AvailableProducts.Add(new SelectListItem { Text = prod.prod_name, Value = prod.prod_id.ToString() });
                }
            }

            return View(model);
        }

        // POST: Updates/Home/SearchClientProduct
        [HttpPost]
        public ActionResult SearchClientProduct(FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add insert logic here
                long cln_id = 0;
                long prod_id = 0;
                foreach (string key in collection.AllKeys)
                {
                    switch (key)
                    {
                        case "ClientId": cln_id = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                        case "ProductId": prod_id = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                    }
                }

                //return RedirectToAction("Index");
                return RedirectToAction("ShowClientProduct", new { idClient = cln_id, idProduct = prod_id, auth = ViewBag.AuthCode });
            }
            catch
            {
                return View();
            }
        }

        // GET: Updates/Home/ShowClientProduct
        public ActionResult ShowClientProduct(long idClient, long idProduct)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            List<UpdateCountModel> updates = new List<UpdateCountModel>();
            string spage = Request["page"] ?? "1";
            string sasc = Request["asc"] ?? "0";
            int page = 1;
            int asc = 0;
            int totalPages = 1;
            if (!int.TryParse(spage, out page) || page < 1) page = 1;
            if (!int.TryParse(sasc, out asc)) asc = 0;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.Ascending = asc != 0;
            using (var access = Factory.GetUpdateAccess())
            {
                updates = access.GetUpdatesForPaging(idClient, idProduct, page, out totalPages, 25, asc != 0);
            }

            return View(updates);
        }

        // GET: Updates/Home/ShowDetails
        public ActionResult ShowDetails()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            string appName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];
            ViewBag.AppName = string.IsNullOrWhiteSpace(appName) ? "" : $"/{appName}";
            ViewBag.StartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            ViewBag.EndDate = DateTime.Now.ToString("yyyy-MM-dd");

            return View();
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
