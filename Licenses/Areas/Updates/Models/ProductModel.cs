using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Updates.Models
{
    public class ProductModel
    {
        #region ctors

        public ProductModel() { }

        public ProductModel(IDataReader rdr)
        {
            prod_id = (long)rdr["prod_id"];
            prod_name = rdr["prod_name"] is System.DBNull ? "?" : rdr["prod_name"].ToString();
        }

        #endregion ctors

        #region Database Fields

        public long prod_id { get; set; }

        public string prod_name { get; set; }

        #endregion Database Fields

        #region View Fields

        [DisplayName("Latest Update")]
        public DateTime LatestUpdate
        {
            get
            {
                DateTime val = DateTime.MinValue;
                foreach (var upd in Updates)
                    if (upd.upd_date > val)
                        val = upd.upd_date;
                return val;
            }
        }

        public List<UpdateCountModel> Updates { get; set; } = new List<UpdateCountModel>();

        #endregion View Fields
    }
}