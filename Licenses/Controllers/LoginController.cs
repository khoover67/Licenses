using Licenses.Models;
using System.Configuration;
using System.Web.Mvc;

namespace Licenses.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // GET: Login
        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            this.TryValidateModel(model);
            if (!ModelState.IsValid)
                return View(model);

            ViewBag.XName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];

            if (model.Name != ConfigurationManager.AppSettings["User"] ||
                model.Password != ConfigurationManager.AppSettings["Password"])
            {
                ModelState.AddModelError(string.Empty, "Login Failed: User or password is invalid.");
                return View(model);
            }

            string token = DataAccess.Auth.GetToken(model.Name);
            //return RedirectToAction("Index", "Client", new { auth = token });
            //return RedirectToAction("Index", "Home", new { area = "Updates", auth = token });
            return RedirectToAction("SearchMethod", "Home", new { area = "Updates", auth = token });
        }
    }
}
