using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Models
{
    public class UnitModel
    {
        public UnitModel()
        {

        }

        public UnitModel(System.Data.IDataReader rdr)
        {
            unit_id = (long)rdr["unit_id"];
            unit_name = rdr["unit_name"] is System.DBNull ? "" : rdr["unit_name"].ToString();
        }

        public UnitModel(FormCollection collection)
        {
            Update(collection);
        }

        [DisplayName("Unit Id")]
        public long unit_id { get; set; }

        [Required(ErrorMessage = "Unit Name is required")]
        [DisplayName("Unit Name")]
        public string unit_name { get; set; }

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "unit_name": unit_name = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                }
            }
        }
    }
}