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
    public class ProductController : Controller
    {
        string CurrentUser { get; set; }

        // GET: Tables/Product
        public ActionResult Index()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            List<ProductModel> models = new List<ProductModel>();
            using (var access = Factory.GetTableAccess())
            {
                models = access.GetProducts();
            }

            return View(models);
        }

        // GET: Tables/Product/Details/5
        public ActionResult Details(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            ProductModel model = new ProductModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(model);

                model = access.GetProduct(id);
            }

            return View(model);
        }

        // GET: Tables/Product/Create
        public ActionResult Create()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            ProductModel model = new ProductModel();
            return View(model);
        }

        // POST: Tables/Product/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add insert logic here
                ProductModel product = new ProductModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    if (access.GetProductId(product.prod_name) > -1)
                    {
                        ModelState.AddModelError(string.Empty, "A product with the name '" + product.prod_name + "' already exists");
                        return View(product);
                    }

                    TryValidateModel(product);
                    if (!ModelState.IsValid)
                        return View(product);

                    int cnt = access.AddProduct(product);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/Product/Edit/5
        public ActionResult Edit(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            ProductModel account = new ProductModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(account);
                account = access.GetProduct(id);
            }

            return View(account);
        }

        // POST: Tables/Product/Edit/5
        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            try
            {
                // TODO: Add update logic here
                ProductModel product = new ProductModel(collection);
                using (var access = Factory.GetTableAccess())
                {
                    if (!CheckExists(access, id))
                        return View(product);

                    TryValidateModel(product);
                    if (!ModelState.IsValid)
                        return View(product);

                    long idCopy = access.GetProductId(product.prod_name);
                    if (idCopy > -1 && idCopy != id)
                    {
                        ModelState.AddModelError(string.Empty, "A product with the name '" + (product.prod_name ?? "") + "' already exists");
                        return View(product);
                    }

                    int cnt = access.UpdateProduct(id, collection);
                }

                return RedirectToAction("Index", new { auth = ViewBag.AuthCode });
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return Content("<html><pre>" + ex.Message + "\r\n\r\n" + ex.ToString() + "</pre></html>");
            }
        }

        // GET: Tables/Product/Delete/5
        public ActionResult Delete(long id)
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

            ProductModel account = new ProductModel();
            using (var access = Factory.GetTableAccess())
            {
                if (!CheckExists(access, id))
                    return View(account);
                account = access.GetProduct(id);
            }

            return View(account);
        }

        // POST: Tables/Product/Delete/5
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
                    int cnt = access.DeleteProduct(id);
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
            var account = access.GetProduct(id);
            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "No Product with an Id of " + id + " was found.");
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
