using Licenses.Areas.Settings.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Settings.DataAccess.SQLite
{
    public class SettingAccess : ISettingAccess
    {
        SQLiteConnection _access = null;
        DateTime _dateOffset = DateTime.Parse("01/01/1970 00:00:00");

        public SettingAccess()
        {
            string file = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/"), "licenses.db");
            if (!File.Exists(file))
                throw new ApplicationException($"The database file '{file}' was not found");

            string connect = $"Data Source={file};Version=3;";
            _access = new SQLiteConnection(connect);
            _access.Open();
        }

        //public int AddSetting(SettingModel model)
        //{
        //    if (model == null) throw new ArgumentNullException("Empty model passed to AddSetting()");
        //    if (string.IsNullOrWhiteSpace(model.set_name)) throw new ArgumentException("model with empty name passed to AddSetting()");
        //    if (string.IsNullOrWhiteSpace(model.set_value)) throw new ArgumentException("model with empty value passed to AddSetting()");
        //}

        //public int DeleteSetting(string name)
        //{

        //}

        //public SettingModel GetSetting()
        //{

        //}

        //public List<SettingModel> GetSettings()
        //{

        //}

        //public int UpdateSetting(string name)
        //{

        //}

        #region IDisposble

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion IDisposble
    }
}