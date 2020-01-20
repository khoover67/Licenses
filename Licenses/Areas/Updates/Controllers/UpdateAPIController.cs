using Licenses.Areas.Updates.DataAccess;
using Licenses.Areas.Updates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Licenses.Areas.Updates.Controllers
{
    public class UpdateAPIController : ApiController
    {
        public string CurrentUser { get; set; }

        // GET: api/UpdateAPI
        public IHttpActionResult Get()
        {
            JsonData data = GetJsonData();
            return Json(data);
        }

        // GET: api/UpdateAPI/5
        public string Get(int id)
        {
            return $"value = {id}";
        }

        /* Post, Put & Delete
        // POST: api/UpdateAPI
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/UpdateAPI/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/UpdateAPI/5
        public void Delete(int id)
        {
        }
        */

        JsonData GetJsonData()
        {
            JsonData data = new JsonData();
            DateTime startDate = DateTime.Today.AddDays(-7);
            DateTime endDate = DateTime.Today;
            DateTime dt;
            string token = "";

            foreach (var parameter in Request.GetQueryNameValuePairs())
            {
                var key = parameter.Key;
                var value = parameter.Value;
                if (key == "StartDate")
                    startDate = DateTime.TryParse(value, out dt) ? dt : startDate;
                else if (key == "EndDate")
                    endDate = DateTime.TryParse(value, out dt) ? dt : endDate;
                else if (key == "auth")
                    token = value;
            }

            if (!ValidateToken(token))
                return data;

            endDate = DateTime.Parse($"{endDate.Month}/{endDate.Day}/{endDate.Year} 23:59:59");
            startDate = DateTime.Parse($"{startDate.Month}/{startDate.Day}/{startDate.Year} 00:00:00");

            if (endDate < startDate)
            {
                dt = endDate;
                endDate = startDate;
                startDate = dt;
            }

            using (var access = Factory.GetUpdateAccess())
            {
                data.data = access.GetUpdateDetailsForJson(startDate, endDate); 
            }

            return data;
        }

        class JsonData
        {
            public List<List<string>> data { get; set; } = new List<List<string>>();
        }

        bool ValidateToken(string token)
        {
            CurrentUser = Licenses.DataAccess.Auth.IsValidToken(token);
            //ViewBag.AuthCode = Licenses.DataAccess.Auth.GetToken(CurrentUser);
            return (!string.IsNullOrWhiteSpace(CurrentUser));
        }
    }
}
