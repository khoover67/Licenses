using Licenses.Library.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Licenses.Library.Server
{
    public static class Tools
    {
        public static ResultDTO PostToServer(UpdateCountDTO dto, string url, bool checkInterval)
        {
            if (dto is null)
                throw new ArgumentNullException("dto passed to PostToServer is empty");
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url passed to PostToServer is empty");

            string jsonRequest = JsonConvert.SerializeObject(dto);
            return PostInfoToServer(jsonRequest, url, checkInterval);
        }

        public static ResultDTO PostInfoToServer(string jsonRequest, string url, bool checkInterval)
        {
            string jsonResponse = "";

            using (var client = new HttpClient())
            {
                // Ignore unauthorized certificate certificate. Make this appSetting?
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // Increase timeout for server response
                TimeSpan timeout = new TimeSpan(0, 30, 0);
                client.Timeout = timeout;

                // Post request to the server
                Task<HttpResponseMessage> task1 = client.PostAsync(
                    url,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                // Get server response
                task1.Wait();
                HttpResponseMessage response = task1.Result;

                // Get string content of response
                Task<string> task2 = response.Content.ReadAsStringAsync();
                task2.Wait();
                jsonResponse = task2.Result ?? "";

                // Try to serialize result to our DTO
                ResultDTO result = JsonConvert.DeserializeObject<ResultDTO>(jsonResponse);
                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                {
                    Exception inner = string.IsNullOrWhiteSpace(result.ErrorStack) ? null : new Exception(result.ErrorStack);
                    if (inner == null)
                        throw new ApplicationException("Error returned from server; " + result.ErrorMessage);
                    else
                        throw new ApplicationException("Error returned from server; " + result.ErrorMessage, inner);
                }

                return result;
            }
        }
        
        public static bool ProductRequiresUpdate(string url, string client, string database, string product)
        {
            using (var http = new HttpClient())
            {
                // Ignore unauthorized certificate certificate. Make this appSetting?
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // Increase timeout for server response
                TimeSpan timeout = new TimeSpan(0, 30, 0);
                http.Timeout = timeout;
                return ProductRequiresUpdate(http, url, client, database, product);
            }
        }

        public static bool ProductRequiresUpdate(HttpClient http, string url, string client, string database, string product)
        {
            // Ignore unauthorized certificate certificate. Make this appSetting?
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // Increase timeout for server response
            TimeSpan timeout = new TimeSpan(0, 30, 0);
            http.Timeout = timeout;

            // Query string
            string query = "?client=" + WebUtility.UrlEncode(client ?? "x") + "&database=" + WebUtility.UrlEncode(database ?? "x") + "&product=" + WebUtility.UrlEncode(product ?? "x");
            string cmd = url + query;

            Task<HttpResponseMessage> task1 = http.GetAsync(cmd);

            // Get server response
            task1.Wait();
            HttpResponseMessage response = task1.Result;

            // Get string content of response
            Task<string> task2 = response.Content.ReadAsStringAsync();
            task2.Wait();
            string serverResponse = task2.Result ?? "";

            if (serverResponse != "\"False\"")
                return true;

            return false;
        }

        //public static ResultDTO PostToServer(UpdateCountDTO client, string url, bool filterByInterval = true)
        //{
        //    if (client is null)
        //        throw new ArgumentNullException("client passed to PostToServer is empty (2)");
        //    if (client.Client is null)
        //        throw new ArgumentNullException("client passed to PostToServer with empty model");
        //    if (string.IsNullOrWhiteSpace(url))
        //        throw new ArgumentNullException("url passed to PostToServer is empty (2)");

        //    if (filterByInterval)
        //    {
        //        if (!FilterProductsByInterval(client.Client, url))
        //        {
        //            System.Diagnostics.Debug.Print("Skipped update");
        //            return new ResultDTO { Message = ResultDTO.INTERVAL };
        //        }
        //    }

        //    string jsonRequest = JsonConvert.SerializeObject(client);
        //    return PostInfoToServer(jsonRequest, url);
        //}

        //public static bool FilterProductsByInterval(ClientModel model, string url)
        //{
        //    if (model == null || model.Products == null || model.Products.Count < 1)
        //        return false;

        //    using (var client = new HttpClient())
        //    {
        //        // Ignore unauthorized certificate certificate. Make this appSetting?
        //        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        //        // Increase timeout for server response
        //        TimeSpan timeout = new TimeSpan(0, 30, 0);
        //        client.Timeout = timeout;
        //        List<ProductModel> newProds = new List<ProductModel>();

        //        foreach (var product in model.Products)
        //        {
        //            string serverResponse = "";

        //            // Query string
        //            string query = "?client=" + WebUtility.UrlEncode(model.Name ?? "x") + "&database=" + WebUtility.UrlEncode(model.DatabasePath ?? "x") + "&product=" + WebUtility.UrlEncode(product.Name ?? "x");
        //            string cmd = url + query;

        //            Task<HttpResponseMessage> task1 = client.GetAsync(cmd);

        //            // Get server response
        //            task1.Wait();
        //            HttpResponseMessage response = task1.Result;

        //            // Get string content of response
        //            Task<string> task2 = response.Content.ReadAsStringAsync();
        //            task2.Wait();
        //            serverResponse = task2.Result ?? "";

        //            if (serverResponse != "\"False\"")
        //                newProds.Add(product);
        //        }

        //        if (newProds.Count != model.Products.Count)
        //            model.Products = newProds;
        //    }

        //    return model.Products.Count > 0;
        //}
    }
}
