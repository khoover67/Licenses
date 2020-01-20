using Licenses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace Licenses.Models
{
    public class ProductDetailModel
    {
        public ProductDetailModel()
        {
        }

        public ProductDetailModel(FormCollection collection)
            : this()
        {
            Update(collection);
        }

        public ProductDetailModel(IDataReader rdr)
            : this()
        {
            PrimaryKey = (int)(long)rdr["PrimaryKey"];
            UpdateId = (int)(long)rdr["UpdateId"];
            Name = rdr["Name"]?.ToString();
            Count = (int)(long)rdr["Count"];
        }

        [DisplayName("Primary Key")]
        public int PrimaryKey { get; set; }

        [DisplayName("Update Id")]
        public int UpdateId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Count is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter positive, integer value")]
        public int Count { get; set; }

        #region UI stuff

        [DisplayName("Client Id")]
        public int ClientId { get; set; }

        [DisplayName("Client Name")]
        public string ClientName { get; set; }

        [DisplayName("Product Id")]
        public int ProductId { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DisplayName("Report Date")]
        public DateTime ReportDate { get; set; }

        public string AuthToken { get; set; }

        #endregion UI stuff

        public List<string> AllDetails { get; set; } = new List<string>();

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "Name": Name = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                    case "Count": Count = (int)collection.GetValue(key).ConvertTo(typeof(int)); break;
                    case "PrimaryKey": PrimaryKey = (int)collection.GetValue(key).ConvertTo(typeof(int)); break;
                }
            }
        }
    }
}