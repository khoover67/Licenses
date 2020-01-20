using Licenses.DataAccess;
//using Licenses.Library.DTO;
using Licenses.Models;
using Licenses.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Licenses.Controllers
{
    //public class ClientAPIController : ApiController
    //{
    //    // GET: api/ClientAPI
    //    public string Get()
    //    {
    //        try
    //        {
    //            return ReportingDue();
    //        }
    //        catch(Exception ex)
    //        {
    //            return ex.Message + "\r\n" + ex.ToString();
    //        }
    //    }

    //    // POST: api/ClientAPI
    //    public ResultDTO Post()
    //    {
    //        ResultDTO result;
    //        try
    //        {
    //            WebApiConfig.SetLogPath();

    //            string json = GetRequestText(Request);
    //            Logger.FileEntry("ClientAPIController.Post\r\n" + json);

    //            ClientDTO dto = JsonConvert.DeserializeObject<ClientDTO>(json);
    //            Licenses.Library.Client.Auth.ValidateDTO(dto);

    //            ClientModel model = dto.Client;
    //            if (model == null)
    //                throw new ApplicationException("Empty ClientModel posted to ClientAPIController");
    //            using (IClientAccess access = Factory.GetClientAccess())
    //            {
    //                Guid id = access.GetClientId(model.Name, model.DatabasePath);
    //                if (id == Guid.Empty)
    //                {
    //                    access.AddClient(model);
    //                }
    //                else
    //                {
    //                    foreach(var product in model.Products)
    //                    {
    //                        product.LastUpdate = DateTime.Now;
    //                        access.AddProduct(id, product);
    //                    }
    //                }
    //            }

    //            result = new ResultDTO { Message = "Success" };
    //        }
    //        catch (Exception ex)
    //        {
    //            result = new ResultDTO { ErrorMessage = ex.Message, ErrorStack = ex.ToString(), Message = "Error" };
    //            Logger.Entry(ex);
    //        }

    //        return result;
    //    }

    //    string GetRequestText(HttpRequestMessage request)
    //    {
    //        Task<string> task = request.Content.ReadAsStringAsync();
    //        task.Wait();
    //        return task.Result;
    //    }

    //string ReportingDue()
    //{
    //    string client = null;
    //    string database = null;
    //    string product = null;

    //    foreach (KeyValuePair<string, string> kvp in Request.GetQueryNameValuePairs())
    //    {
    //        switch (kvp.Key.ToLower())
    //        {
    //            case "client":
    //                client = kvp.Value.Trim();
    //                break;
    //            case "database":
    //                database = kvp.Value.Trim();
    //                break;
    //            case "product":
    //                product = kvp.Value.Trim();
    //                break;
    //        }
    //    }

    //    if (string.IsNullOrWhiteSpace(client))
    //        return "No client provided";

    //    if (string.IsNullOrWhiteSpace(database))
    //        return "No database provided";

    //    if (string.IsNullOrWhiteSpace(product))
    //        return "No product provided";

    //    DateTime lastModified;
    //    using (var access = Factory.GetClientAccess())
    //    {
    //        lastModified = access.LastProductUpdate(client, database, product);
    //    }

    //    int interval = 1;
    //    using (var settings = Factory.GetSettingAccess())
    //    {
    //        SettingModel val = settings.GetSetting(SettingModel.ReportInterval);
    //        if (val == null || !int.TryParse(val.set_value, out interval))
    //            interval = 1;
    //    }

    //    if (interval < 1 || interval > 7)
    //        interval = 1;

    //    DateTime nextUpdate = lastModified.AddDays(interval);
    //    TimeSpan ts = nextUpdate.Subtract(DateTime.Now);
    //    if (ts.TotalDays <= 0)
    //        return "True";

    //    return "False";
    //}
}
