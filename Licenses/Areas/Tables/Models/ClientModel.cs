using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Models
{
    public class ClientModel
    {
        public ClientModel()
        {

        }

        public ClientModel(System.Data.IDataReader rdr)
        {
            cln_id = (long)rdr["cln_id"];
            cln_name = rdr["cln_name"] is System.DBNull ? "" : rdr["cln_name"].ToString();
            cln_db_path = rdr["cln_db_path"] is System.DBNull ? "" : rdr["cln_db_path"].ToString();
            cln_account_id = rdr["cln_account_id"] is DBNull ? null : (long?)rdr["cln_account_id"];
        }

        public ClientModel(FormCollection collection)
        {
            Update(collection);
        }

        [DisplayName("Client Id")]
        public long cln_id { get; set; }

        [Required(ErrorMessage = "Client Name is required")]
        [DisplayName("Client Name")]
        public string cln_name { get; set; }

        [Required(ErrorMessage = "Database Path is required")]
        [DisplayName("Database Path")]
        public string cln_db_path { get; set; }

        [DisplayName("Account Id")]
        public long? cln_account_id { get; set; }

        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }

        public List<SelectListItem> AvailableAccounts { get; set; } = new List<SelectListItem>(); 

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "cln_name": cln_name = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                    case "cln_db_path": cln_db_path = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                    case "cln_account_id": cln_account_id = (long?)collection.GetValue(key).ConvertTo(typeof(long?)); break;
                }
            }
        }
    }
}