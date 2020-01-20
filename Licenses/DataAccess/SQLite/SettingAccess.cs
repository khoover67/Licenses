using Licenses.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.DataAccess.SQLite
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

        public List<SettingModel> GetSettings()
        {
            List<SettingModel> result = new List<SettingModel>();
            bool bInterval = false;
            string sql =
                "select * \r\n" +
                "  from setting \r\n" +
                " order by set_name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while(rdr.Read())
                    {
                        SettingModel model = new SettingModel(rdr);
                        result.Add(model);
                        if (!bInterval)
                            if (model.set_name == SettingModel.ReportInterval)
                                bInterval = true;
                    }
                }

                if (!bInterval)
                    result.Add(new SettingModel { set_name = SettingModel.ReportInterval, set_value = "1" });
            }

            return result;
        }

        public SettingModel GetSetting(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Empty name passed to GetSetting()");

            SettingModel result = null;
            string sql =
                "select * \r\n" +
                "  from setting \r\n" +
                " where set_name = @name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", name);
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new SettingModel(rdr);
                    }
                }
            }

            return result;
        }

        public int AddSetting(SettingModel setting)
        {
            if (setting == null)
                throw new ArgumentNullException("Empty setting model passed to AddSetting()");
            if (string.IsNullOrWhiteSpace(setting.set_name))
                throw new ArgumentNullException("Model with empty name passed to AddSetting()");
            if (string.IsNullOrWhiteSpace(setting.set_value))
                throw new ArgumentNullException("Model with empty value passed to AddSetting()");

            string sql =
                "insert into setting \r\n" +
                "( \r\n" +
                "   set_name, \r\n" +
                "   set_value \r\n" +
                ")   \r\n" +
                "values   \r\n" +
                "(   \r\n" +
                "   @name, \r\n" +
                "   @value \r\n" +
                ")   \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", setting.set_name);
                cmd.Parameters.AddWithValue("@value", setting.set_value);
                return ExecuteNonQuery(cmd);
            }
        }

        public int DeleteSetting(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Empty name passed to DeleteSetting()");

            string sql =
                "delete from setting \r\n" +
                " where set_name = @name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", name);
                return ExecuteNonQuery(cmd);
            }
        }

        public int UpdateSetting(string name, FormCollection collection)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Empty name passed to UpdateSetting()");

            string set_value = "";
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "set_value": set_value = (string)collection.GetValue(key).ConvertTo(typeof(string)); break;
                }
            }

            string sql =
                "update setting \r\n" +
                "   set set_value = @value \r\n" +
                " where set_name = @name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@value", set_value);
                cmd.Parameters.AddWithValue("@name", name);
                return ExecuteNonQuery(cmd);
            }
        }

        #region Data Access

        SQLiteDataReader ExecuteReader(SQLiteCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("Empty command sent to SQLite.ExecuteReader");

            string sql = cmd.CommandText ?? "<No query text>";
            try
            {
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                try
                {
                    if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                    {
                        for (int i = 0; i < cmd.Parameters.Count; i++)
                        {
                            sql += "\r\n  '" + (cmd.Parameters[i].ParameterName ?? "") + "' = '" + cmd.Parameters[i].Value?.ToString() + "'";
                        }
                    }
                }
                catch { }
                string msg = "Failed to execute reader: " + ex.Message + "\r\n" + sql + "\r\n\r\n";
                Tools.Logger.Entry(msg + ex.ToString(), true, EventLogEntryType.Error);
                throw new ApplicationException(msg, ex);
            }
        }

        int ExecuteNonQuery(SQLiteCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("Empty command sent to SQLite.ExecuteNonQuery");

            string sql = cmd.CommandText ?? "<No query text>";
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                try
                {
                    if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                    {
                        for (int i = 0; i < cmd.Parameters.Count; i++)
                        {
                            sql += "\r\n  '" + (cmd.Parameters[i].ParameterName ?? "") + "' = '" + cmd.Parameters[i].Value?.ToString() + "'";
                        }
                    }
                }
                catch { }
                string msg = "Failed to execute non query: " + ex.Message + "\r\n" + sql + "\r\n\r\n";
                Tools.Logger.Entry(msg + ex.ToString(), true, EventLogEntryType.Error);
                throw new ApplicationException("Failed to execute non query: " + ex.Message + "\r\n" + sql + "\r\n", ex);
            }
        }

        DateTime LongToDate(long secs)
        {
            DateTime result = _dateOffset.AddSeconds(secs);
            return result;
        }

        long DateToLong(DateTime date)
        {
            TimeSpan ts = date.Subtract(_dateOffset);
            return (long)Math.Round(ts.TotalSeconds);
        }

        #endregion Data Access

        #region IDispose

        public void Dispose()
        {
            // Not used
        }

        #endregion IDispose
    }
}