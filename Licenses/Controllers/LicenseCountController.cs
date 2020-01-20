using Licenses.DataAccess;
using Licenses.Library.DTO;
using Licenses.Models;
using Licenses.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Licenses.Controllers
{
    public class LicenseCountController : ApiController
    {
        // GET:  api/LicenseCount
        public string Get()
        {
            try
            {
                return ReportingDue();
            }
            catch (Exception ex)
            {
                return ex.Message + "\r\n" + ex.ToString();
            }
        }

        // POST: api/LicenseCount
        public ResultDTO Post()
        {
            ResultDTO result = null;
            try
            {
                WebApiConfig.SetLogPath();

                string json = GetRequestText(Request);
                Logger.FileEntry("**** LicenseCount Post ****\r\n" + json + "\r\n***************************\r\n");

                UpdateCountDTO dto = JsonConvert.DeserializeObject<UpdateCountDTO>(json);
                Library.Encryption.Auth.ValidateDTO(dto);

                SortedList<string, int> units = new SortedList<string, int>();
                foreach(var unit in dto.Units)
                {
                    if (units.TryGetValue(unit.UnitName, out int cnt))
                        units[unit.UnitName] = cnt + unit.UnitCount;
                    else
                        units.Add(unit.UnitName, unit.UnitCount);
                }

                using (ILicensesAccess access = Factory.GetLicensesAccess())
                {
                    long cnt = access.AddUpdateCount(DateTime.Now, dto.ClientName, dto.DatabasePath, dto.ProductName, units, dto.Date);
                }

                result = new ResultDTO { Message = "Success" };
            }
            catch (Exception ex)
            {
                result = new ResultDTO { ErrorMessage = ex.Message, ErrorStack = ex.ToString(), Message = "Error" };
                Logger.Entry(ex);
            }

            return result;
        }

        string ReportingDue()
        {
            string client = null;
            string database = null;
            string product = null;

            foreach (KeyValuePair<string, string> kvp in Request.GetQueryNameValuePairs())
            {
                switch (kvp.Key.ToLower())
                {
                    case "client":
                        client = kvp.Value.Trim();
                        break;
                    case "database":
                        database = kvp.Value.Trim();
                        break;
                    case "product":
                        product = kvp.Value.Trim();
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(client))
                return "No client provided";

            if (string.IsNullOrWhiteSpace(database))
                return "No database provided";

            if (string.IsNullOrWhiteSpace(product))
                return "No product provided";

            DateTime lastModified;
            using (var access = Factory.GetLicensesAccess())
            {
                lastModified = access.LastUpdate(client, database, product);
            }

            int interval = 1;
            using (var settings = Factory.GetSettingAccess())
            {
                SettingModel val = settings.GetSetting(SettingModel.ReportInterval);
                if (val == null || !int.TryParse(val.set_value, out interval))
                    interval = 1;
            }

            if (interval < 1 || interval > 7)
                interval = 1;

            DateTime nextUpdate = lastModified.AddDays(interval);
            TimeSpan ts = nextUpdate.Subtract(DateTime.Now);
            if (ts.TotalDays <= 0)
                return "True";

            return "False";
        }

        string GetRequestText(HttpRequestMessage request)
        {
            Task<string> task = request.Content.ReadAsStringAsync();
            task.Wait();
            return task.Result;
        }
    }
}
