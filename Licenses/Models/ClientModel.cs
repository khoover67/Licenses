using Licenses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace Licenses.Models
{
    public class ClientModel
    {
        public ClientModel()
        {
        }

        public ClientModel(FormCollection collection)
            : this()
        {
            Update(collection);
        }

        public ClientModel(IDataReader rdr)
            : this()
        {
            PrimaryKey = (int)(long)rdr["PrimaryKey"];
            Name = rdr["Name"]?.ToString();
            DatabasePath = rdr["DatabasePath"]?.ToString();
        }

        [DisplayName("Id")]
        public int PrimaryKey { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [DisplayName("Database Path")]
        [Required(ErrorMessage = "Database Path is required")]
        public string DatabasePath { get; set; }

        public List<ProductModel> Products { get; private set; } = new List<ProductModel>();

        [DisplayName("Product Count")]
        public int ProductCount
        {
            get { return Products.Count;  }
        }

        public void AddProduct(ProductModel product)
        {
            if (product != null)
            {
                product.ClientId = PrimaryKey;
                product.ClientName = Name;
                Products.Add(product);
            }
        }

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "Name": Name = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                    case "DatabasePath": DatabasePath = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                    case "PrimaryKey": PrimaryKey = (int)collection.GetValue(key).ConvertTo(typeof(int)); break;
                }
            }
        }
    }
}