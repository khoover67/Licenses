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
    public class SettingModel
    {
        public static readonly string ReportInterval = "Reporting Interval";

        public SettingModel()
        {
        }

        public SettingModel(IDataReader rdr)
        {
            set_name = rdr["set_name"] is System.DBNull ? "<blank>" : rdr["set_name"].ToString();
            set_value = rdr["set_value"] is System.DBNull ? "<blank>" : rdr["set_value"].ToString();
        }

        public SettingModel(FormCollection collection)
            : this()
        {
            Update(collection);
        }

        [DisplayName("Setting Name")]
        [Required(ErrorMessage = "Name is required")]
        public string set_name { get; set; }

        [DisplayName("Setting Value")]
        [Required(ErrorMessage = "Value is required")]
        public string set_value { get; set; }

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "set_name": set_name = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                    case "set_value": set_value = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                }
            }
        }
    }
}