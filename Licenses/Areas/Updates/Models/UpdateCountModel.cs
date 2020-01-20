using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Updates.Models
{
    public class UpdateCountModel
    {
        public UpdateCountModel() { }

        //public UpdateCountModel(IDataReader rdr)
        //{
        //    upd_id = (long)rdr["upd_id"];
        //    upd_date = (DateTime)rdr["upd_date"];
        //    upd_client_id = (long)rdr["upd_client_id"];
        //    upd_product_id = (long)rdr["upd_product_id"];
        //    upd_count = (long)rdr["upd_count"];
        //}

        #region Database Fields

        public long upd_id { get; set; }

        [DisplayName("Date")]
        public DateTime upd_date { get; set; }

        public long upd_client_id { get; set; }

        public long upd_product_id { get; set; }

        [DisplayName("Count")]
        public long upd_count { get; set; }

        #endregion Database Fields

        #region View Fields

        public ClientModel Client { get; set; }

        [DisplayName("Client Name")]
        public string ClientName { get { return Client != null ? Client.cln_name ?? "--" : "**"; } }

        public ProductModel Product { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get { return Product != null ? Product.prod_name ?? "--" : "**"; } }

        #endregion View Fields
    }
}