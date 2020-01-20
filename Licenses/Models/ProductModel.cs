using Licenses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace Licenses.Models
{
    public class ProductModel
    {
        public ProductModel()
        {
        }

        public ProductModel(FormCollection collection)
            : this()
        {
            Update(collection);
        }

        public ProductModel(IDataReader rdr)
            : this()
        {
            PrimaryKey = (int)(long)rdr["PrimaryKey"];
            Name = rdr["Name"].ToString();
            ClientId = (int)(long)rdr["ClientId"];
        }

        [DisplayName("Id")]
        public int PrimaryKey { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [DisplayName("Client Id")]
        public int ClientId { get; set; }

        public List<ProductUpdateModel> Updates { get; private set; } = new List<ProductUpdateModel>();

        [DisplayName("Client Name")]
        public string ClientName { get; set; }

        [DisplayName("Last Update")]
        public DateTime LastUpdate { get; set; }

        [DisplayName("Total Updates")]
        public int TotalUpdates { get; set; } = 0;

        public string AuthToken { get; set; }

        public List<string> AllProducts { get; set; } = new List<string>();

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "Name": Name = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                    case "ClientId": ClientId = (int)collection.GetValue(key).ConvertTo(typeof(int)); break;
                    case "PrimaryKey": PrimaryKey = (int)collection.GetValue(key).ConvertTo(typeof(int)); break;
                }
            }
        }

        public void AddUpdate(ProductUpdateModel update)
        {
            if (update != null)
            {
                update.ClientId = ClientId;
                update.ClientName = ClientName;
                update.ProductName = Name;
                Updates.Add(update);
            }
        }
    }
}