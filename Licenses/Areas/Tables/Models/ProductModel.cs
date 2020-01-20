using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Models
{
    public class ProductModel
    {
        public ProductModel()
        {

        }

        public ProductModel(System.Data.IDataReader rdr)
        {
            prod_id = (long)rdr["prod_id"];
            prod_name = rdr["prod_name"] is System.DBNull ? "" : rdr["prod_name"].ToString();
        }

        public ProductModel(FormCollection collection)
        {
            Update(collection);
        }

        [DisplayName("Product Id")]
        public long prod_id { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [DisplayName("Product Name")]
        public string prod_name { get; set; }

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "prod_name": prod_name = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                }
            }
        }
    }
}