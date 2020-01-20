using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Models
{
    public class UpdateCountModel
    {
        public UpdateCountModel()
        {

        }

        public UpdateCountModel(System.Data.IDataReader rdr)
        {
            upd_id = (long)rdr["upd_id"];
            upd_date = (long)rdr["upd_date"];
            upd_client_id = (long)rdr["upd_client_id"];
            upd_product_id = (long)rdr["upd_product_id"];
            upd_count = rdr["upd_count"] is System.DBNull ? 0 : (long)rdr["upd_count"];
        }

        public UpdateCountModel(FormCollection collection)
        {
            Update(collection);
        }

        #region Database Properties

        [DisplayName("Update Id")]
        public long upd_id { get; set; }

        [DisplayName("Update Date as Long")]
        public long upd_date { get; set; }

        [Required(ErrorMessage = "Client is required")]
        [DisplayName("Client Id")]
        public long upd_client_id { get; set; }

        [Required(ErrorMessage = "Product is required")]
        [DisplayName("Product Id")]
        public long upd_product_id { get; set; }

        [Required(ErrorMessage = "Update Count is required")]
        [DisplayName("Update Count")]
        public long upd_count { get; set; }

        #endregion Database Properties

        #region UI Properties

        [Required(ErrorMessage = "Update Date is required")]
        [DisplayName("Update Date")]
        public DateTime UpdateDate { get; set; }

        [DisplayName("Client Name")]
        public string ClientName { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        public List<SelectListItem> AvailableClients { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> AvailableProducts { get; set; } = new List<SelectListItem>();

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "UpdateDate": UpdateDate = (DateTime)collection.GetValue(key).ConvertTo(typeof(DateTime)); break;
                    case "upd_client_id": upd_client_id = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                    case "upd_product_id": upd_product_id = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                  //case "upd_count": upd_count = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                }
            }
        }

        public List<UpdateUnitModel> Units { get; set; } = new List<UpdateUnitModel>();

        #endregion UI Properties
    }
}