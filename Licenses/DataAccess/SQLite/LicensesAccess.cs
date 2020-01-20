using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Licenses.Models;

namespace Licenses.DataAccess.SQLite
{
    public class LicensesAccess : ILicensesAccess
    {
        SQLiteConnection _access = null;
        DateTime _dateOffset = DateTime.Parse("01/01/1970 00:00:00");

        public LicensesAccess()
        {
            string file = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/"), "licenses.db");
            if (!File.Exists(file))
                throw new ApplicationException($"The database file '{file}' was not found");

            string connect = $"Data Source={file};Version=3;";
            _access = new SQLiteConnection(connect);
            _access.Open();
        }

        public long AddUpdateCount(DateTime dateTime, string clientName, string dbPath, string productName, SortedList<string, int> units, DateTime universalDate)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentNullException("Empty clientName passed to AddUpdateCount()");
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentNullException("Empty dbPath passed to AddUpdateCount()");
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentNullException("Empty productName passed to AddUpdateCount()");
            if (units == null)
                throw new ArgumentNullException("Null units passed to AddUpdateCount()");
            if (units.Count < 1)
                throw new ArgumentNullException("Empty units passed to AddUpdateCount()");

            DateTime dtLocal = universalDate.ToLocalTime();
            long date = DateToLong(dtLocal);
            long clientId = AddClient(clientName, dbPath);
            long productId = AddProduct(productName);
            SortedList<long, int> units_ids = new SortedList<long, int>();

            if (CountExistsForDay(clientId, productId, dtLocal))
                return 0;

            for (int i = 0; i < units.Count; i++)
            {
                string unit = units.Keys[i];
                int count = units.Values[i];
                long id = AddUnit(unit);
                units_ids.Add(id, count);
            }

            return AddUpdateCount(date, clientId, productId, units_ids);
        }

        public bool CountExistsForDay(long client, long productName, DateTime dtLocal)
        {
            long from = DateToLong(DateTime.Parse($"{dtLocal.Month}/{dtLocal.Day}/{dtLocal.Year} 00:00:00"));
            long to = DateToLong(DateTime.Parse($"{dtLocal.Month}/{dtLocal.Day}/{dtLocal.Year} 11:59:50"));
            long cnt = 0;
            string sql =
                "select count(*) cnt \r\n" +
                "  from update_count \r\n" +
               $" where upd_date >= {from} \r\n" +
               $"   and upd_date <= {to} \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        string result = rdr[0] is System.DBNull ? "0" : rdr[0].ToString();
                        if (!long.TryParse(result, out cnt))
                            cnt = 0;
                    }
                }
            }

            return cnt > 0;
        }

        public DateTime LastUpdate(string clientName, string dbPath, string productName)
        {
            DateTime result = DateTime.MinValue;
            long cln_id = GetClientId(clientName, dbPath);
            long prod_id = GetProductId(productName);
            if (cln_id < 0 || prod_id < 0)
                return result;

            string sql =
                "select max(upd_date) \r\n" +
                "  from update_count \r\n" +
                " where upd_client = " + cln_id + " \r\n" +
                "   and upd_product = " + prod_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        long date = (long)rdr[0];
                        return LongToDate(date);
                    }
                }
            }

            return result;
        }

        #region Helpers

        long AddClient(string clientName, string dbPath, long? idAccount = null)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentNullException("Empty clientName passed to AddClient()");
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentNullException("Empty dbPath passed to AddClient()");

            long id = GetClientId(clientName, dbPath);
            if (id > -1)
                return id;

            string sql;
            if (idAccount.HasValue && idAccount > -1)
            {
                sql =
                    "insert into client \r\n" +
                    "( \r\n" +
                    "   cln_name, \r\n" +
                    "   cln_db_path, \r\n" +
                    "   cln_account_id \r\n" +
                    ") \r\n" +
                    "values \r\n" +
                    "( \r\n" +
                    "  @name, \r\n" +
                    "  @dbPath, \r\n" +
                    "  " + idAccount + " \r\n" +
                    ") \r\n";
            }
            else
            {
                sql =
                    "insert into client \r\n" +
                    "( \r\n" +
                    "   cln_name, \r\n" +
                    "   cln_db_path \r\n" +
                    ") \r\n" +
                    "values \r\n" +
                    "( \r\n" +
                    "  @name, \r\n" +
                    "  @dbPath \r\n" +
                    ") \r\n";
            }

            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", clientName);
                cmd.Parameters.AddWithValue("@dbPath", dbPath);
                ExecuteNonQuery(cmd);
            }

            return GetClientId(clientName, dbPath);
        }

        long AddProduct(string productName)
        {
            long id = GetProductId(productName);
            if (id > -1)
                return id;

            string sql =
                "insert into product \r\n" +
                "( \r\n" +
                "   prod_name \r\n" +
                ") \r\n" +
                "values \r\n" +
                "( \r\n" +
                "   @name \r\n" +
                ") \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", productName);
                ExecuteNonQuery(cmd);
            }

            id = GetProductId(productName);
            return id;
        }

        long AddUnit(string unitName)
        {
            long id = GetUnitId(unitName);
            if (id > -1)
                return id;

            string sql =
                "insert into unit \r\n" +
                "( \r\n" +
                "   unit_name \r\n" +
                ") \r\n" +
                "values \r\n" +
                "( \r\n" +
                "   @name \r\n" +
                ") \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", unitName);
                ExecuteNonQuery(cmd);
            }

            id = GetUnitId(unitName);
            return id;
        }

        long AddUpdateCount(long date, long client_id, long product_id, SortedList<long, int> units_ids)
        {
            int cnt = 0;
            long id = -1;
            string sql =
                "delete from update_count \r\n" +
                " where upd_date = " + date + " \r\n" +
                "   and upd_client_id = " + client_id + " \r\n" +
                "   and upd_product_id = " + product_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                ExecuteNonQuery(cmd);
            }

            sql =
                "insert into update_count \r\n" +
                "( \r\n" +
                "   upd_date, \r\n" +
                "   upd_client_id, \r\n" +
                "   upd_product_id \r\n" +
                ") \r\n" +
                "values \r\n" +
                "( \r\n" +
                "   " + date + ", \r\n" +
                "   " + client_id + ", \r\n" +
                "   " + product_id + " \r\n" +
                ") \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cnt = ExecuteNonQuery(cmd);
            }

            sql =
                "select upd_id \r\n" +
                "  from update_count \r\n" +
               $" where upd_date = {date} \r\n" +
               $"   and upd_client_id = {client_id} \r\n" +
               $"   and upd_product_id = {product_id} \r\n";
            using (var cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        id = (long)rdr["upd_id"];
                    }
                }
            }

            if (id < 0)
                throw new ApplicationException($"Failed to add update_unit records: The query \r\n{sql} returned no record id");

            for(int i = 0; i < units_ids.Count; i++)
            {
                long unit_id = units_ids.Keys[i];
                long count = units_ids.Values[i];
                sql =
                    "insert into update_unit \r\n" +
                    "( \r\n" +
                    "  updunit_upd_id, \r\n" +
                    "  updunit_unit_id, \r\n" +
                    "  updunit_count \r\n" +
                    ") \r\n" +
                    "values \r\n" +
                    "( \r\n" +
                   $"  {id}, \r\n" +
                   $"  {unit_id}, \r\n" +
                   $"  {count} \r\n" +
                    ") \r\n";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
                {
                    cnt += ExecuteNonQuery(cmd);
                }
            }

            return id;
        }

        long GetClientId(string clientName, string dbPath)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentNullException("Empty clientName passed to GetClientId()");
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentNullException("Empty dbPath passed to GetClientId()");

            string sql =
                "select cln_id \r\n" +
                "  from client \r\n" +
                " where cln_name = @name \r\n" +
                "   and cln_db_path = @dbPath \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", clientName ?? "");
                cmd.Parameters.AddWithValue("@dbPath", dbPath ?? "");
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        return (long)rdr[0];
                    }
                }
            }

            return -1;
        }

        long GetProductId(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentNullException("Empty productName passed to GetProductId()");

            string sql =
                "select prod_id \r\n" +
                "  from product \r\n" +
                " where prod_name = @name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", productName ?? "");
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        return (long)rdr[0];
                    }
                }
            }

            return -1;
        }

        long GetUnitId(string unitName)
        {
            if (string.IsNullOrWhiteSpace(unitName))
                throw new ArgumentNullException("Empty unitName passed to GetUnitId()");

            string sql =
                "select unit_id \r\n" +
                "  from unit \r\n" +
                " where unit_name = @name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", unitName ?? "");
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        return (long)rdr[0];
                    }
                }
            }

            return -1;
        }

        #endregion Helpers

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

        public void Dispose()
        {
            if (_access != null)
            {
                try
                {
                    _access.Dispose();
                    _access = null;
                }
                catch { }
            }
        }
    }
}