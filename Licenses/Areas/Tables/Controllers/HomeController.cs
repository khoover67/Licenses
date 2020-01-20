using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Controllers
{
    public class HomeController : Controller
    {
        string CurrentUser { get; set; }

        // GET: Tables/Home
        public ActionResult Index()
        {
            if (!ValidateToken())
                return RedirectToAction("Index", "Login", new { area = "" });

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
