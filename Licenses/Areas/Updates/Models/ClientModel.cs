using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Updates.Models
{
    public class ClientModel
    {
        #region ctors

        public ClientModel() { }

        public ClientModel(IDataReader rdr)
        {
            cln_id = (long)rdr["cln_id"];
            cln_name = rdr["cln_name"] is System.DBNull ? "?" : rdr["cln_name"].ToString();
            cln_db_path = rdr["cln_db_path"] is System.DBNull ? "?" : rdr["cln_db_path"].ToString();
        }

        #endregion ctors

        #region Database Fields

        public long cln_id { get; set; }

        [DisplayName("Client Name")]
        [Required(ErrorMessage = "Client Name is required")]
        public string cln_name { get; set; } = "?";

        [DisplayName("Database Path")]
        [Required(ErrorMessage = "Database Path is required")]
        public string cln_db_path { get; set; }

        #endregion Database Fields

        #region View Fields

        public string HtmlNameId { get { return "client_model_" + cln_id; } }

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

        [DisplayName("Update Count")]
        public long UpdateCount { get; set; }

        public List<UpdateCountModel> Updates { get; set; } = new List<UpdateCountModel>();

        #endregion View Fields

        #region Helper Functions

        // HELPER FUNCTIONS HERE

        #endregion Helper Functions
    }
}