using Licenses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Models
{
    public class ProductUpdateModel
    {
        public ProductUpdateModel()
        {
        }

        public ProductUpdateModel(FormCollection collection)
            : this()
        {
            Update(collection);
        }

        public ProductUpdateModel(IDataReader rdr)
            : this()
        {
            PrimaryKey = (int)(long)rdr["PrimaryKey"];
            ProductId = (int)(long)rdr["ProductId"];
            ReportDate = (DateTime)rdr["ReportDate"];
        }

        public int PrimaryKey { get; set; }

        [DisplayName("Product Id")]
        public int ProductId { get; set; }

        [DisplayName("Report Date")]
        [DataType(DataType.DateTime)]
        public DateTime ReportDate { get; set; }

        public List<ProductDetailModel> Details { get; private set; } = new List<ProductDetailModel>();

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "Id": PrimaryKey = (int)collection.GetValue(key).ConvertTo(typeof(int)); break;
                    case "ProductId": ProductId = (int)collection.GetValue(key).ConvertTo(typeof(int)); break;
                    case "ReportDate": ReportDate = (DateTime)collection.GetValue(key).ConvertTo(typeof(DateTime)); break;
                }
            }
        }

        #region Extra for UI

        public string AuthToken { get; set; }

        [DisplayName("Client Id")]
        public int ClientId { get; set; }

        [DisplayName("Client Name")]
        public string ClientName { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DisplayName("Unit Count")]
        public int UnitCount { get; set; }

        #endregion Extra for UI

        public void AddProductDetail(ProductDetailModel detail)
        {
            detail.ClientId = ClientId;
            detail.ClientName = ClientName;
            detail.ProductName = ProductName;
            Details.Add(detail);
        }
    }
}