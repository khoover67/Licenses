using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Models
{
    public class AccountModel
    {
        public AccountModel()
        {

        }

        public AccountModel(System.Data.IDataReader rdr)
        {
            account_id = (long)rdr["account_id"];
            account_number = (long)rdr["account_number"];
            account_name = rdr["account_name"] is System.DBNull ? "" : rdr["account_name"].ToString();
        }

        public AccountModel(FormCollection collection)
        {
            Update(collection);
        }

        [DisplayName("Account Id")]
        public long account_id { get; set; }

        [Required(ErrorMessage = "Account Number is required")]
        [DisplayName("Account Number")]
        public long account_number { get; set; }

        [Required(ErrorMessage = "Account Name is required")]
        [DisplayName("Account Name")]
        public string account_name { get; set; }

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "account_id": account_id = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                    case "account_number": account_number = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                    case "account_name": account_name = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                }
            }
        }
    }
}