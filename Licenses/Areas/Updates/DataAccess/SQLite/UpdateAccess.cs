using Licenses.Areas.Updates.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Updates.DataAccess.SQLite
{
    public class UpdateAccess : IUpdateAccess
    {
        SQLiteConnection _access = null;
        DateTime _dateOffset = DateTime.Parse("01/01/1970 00:00:00");

        public UpdateAccess()
        {
            string file = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/"), "licenses.db");
            if (!File.Exists(file))
                throw new ApplicationException($"The database file '{file}' was not found");

            string connect = $"Data Source={file};Version=3;";
            _access = new SQLiteConnection(connect);
            _access.Open();
        }

        #region Client

        public AllClientsAndProducts GetAllClientsAndProducts()
        {
            AllClientsAndProducts result = new AllClientsAndProducts();
            string sql =
                "select * \r\n" +
                "  from client \r\n" +
                " order by \r\n" +
                "       cln_name, \r\n" +
                "       cln_db_path \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        ClientModel client = new ClientModel(rdr);
                        result.Clients.Add(client);
                    }
                }
            }

            sql =
                "select * \r\n" +
                "  from product \r\n" +
                " order by \r\n" +
                "       prod_name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        ProductModel product = new ProductModel(rdr);
                        result.Products.Add(product);
                    }
                }
            }

            return result;
        }

        public ClientModel GetClient(long cln_id)
        {
            ClientModel result = null;
            string sql =
                "select *  \r\n" +
                "  from client \r\n" +
                " where cln_id = " + cln_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new ClientModel(rdr);
                    }
                }
            }

            return result;
        }

        public ProductModel GetProduct(long prod_id)
        {
            ProductModel result = null;
            string sql =
                "select *  \r\n" +
                "  from product \r\n" +
                " where prod_id = " + prod_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new ProductModel(rdr);
                    }
                }
            }

            return result;
        }

        public List<ClientModel> GetLatestUpdatesByClient()
        {
            List<ClientModel> result = new List<ClientModel>();
            SortedList<long, UpdateCountModel> updates = new SortedList<long, UpdateCountModel>();
            SortedList<long, ClientModel> clients = new SortedList<long, ClientModel>();
            SortedList<long, ProductModel> products = new SortedList<long, ProductModel>();

            string sql = "DROP TABLE IF EXISTS temp.latestupdate1;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                ExecuteNonQuery(cmd);
            }

            sql =
                "CREATE TEMPORARY TABLE latestupdate1 AS \r\n" +
                "select upd_client_id max_client_id, \r\n" +
                "       upd_product_id max_product_id, \r\n" +
                "       max(upd_date) max_date \r\n" +
                "  from update_count \r\n" +
                " group by \r\n" +
                "       upd_client_id, \r\n" +
                "       upd_product_id \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                ExecuteNonQuery(cmd);
            }

            sql =
                "select * \r\n" +
                "  from client \r\n" +
                " where cln_id in ( \r\n" +
                "       select distinct max_client_id \r\n" +
                "         from temp.latestupdate1 \r\n" +
                "       ) \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while(rdr.Read())
                    {
                        ClientModel client = new ClientModel(rdr);
                        if (!clients.ContainsKey(client.cln_id))
                            clients.Add(client.cln_id, client);
                    }
                }
            }

            sql =
                "select * \r\n" +
                "  from product \r\n" +
                " where prod_id in ( \r\n" +
                "       select distinct max_product_id \r\n" +
                "         from temp.latestupdate1 \r\n" +
                "       ) \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while(rdr.Read())
                    {
                        ProductModel product = new ProductModel(rdr);
                        if (!products.ContainsKey(product.prod_id))
                            products.Add(product.prod_id, product);
                    }
                }
            }

            sql =
                "select update_count.*, \r\n" +
                "       (select sum(updunit_count) from update_unit where updunit_upd_id = upd_id) as upd_count \r\n" +
                "  from update_count \r\n" +
                " INNER JOIN latestupdate1 \r\n" +
                "    ON upd_client_id = max_client_id \r\n" +
                "   AND upd_product_id = max_product_id \r\n" +
                "   AND upd_date = max_date \r\n" +
                " ORDER BY \r\n" +
                "       upd_client_id";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while(rdr.Read())
                    {
                        UpdateCountModel update = new UpdateCountModel();
                        update.upd_id = (long)rdr["upd_id"];
                        update.upd_date = LongToDate((long)rdr["upd_date"]);
                        update.upd_client_id = (long)rdr["upd_client_id"];
                        update.upd_product_id = (long)rdr["upd_product_id"];
                        update.upd_count = rdr["upd_count"] is System.DBNull ? 0 : (long)rdr["upd_count"];

                        if (!updates.ContainsKey(update.upd_id))
                            updates.Add(update.upd_id, update);
                    }
                }
            }

            sql = "DROP TABLE IF EXISTS temp.latestupdate1;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                ExecuteNonQuery(cmd);
            }

            ClientModel cln = null;
            ProductModel prod = null;
            long last_cln = -1;
            foreach(UpdateCountModel upd in updates.Values)
            {
                if (upd.upd_client_id != last_cln)
                {
                    if (!clients.TryGetValue(upd.upd_client_id, out cln))
                        continue;
                    last_cln = upd.upd_client_id;
                }

                upd.Client = cln;

                if (!products.TryGetValue(upd.upd_product_id, out prod))
                    continue;
                upd.Product = prod;

                cln.Updates.Add(upd);
                prod.Updates.Add(upd);
            }
            
            result = clients.Values.ToList();
            if (result.Count > 1)
            {
                result.Sort(delegate (ClientModel c1, ClientModel c2) {
                    int diff = c1.cln_name.CompareTo(c2.cln_name);
                    if (diff == 0)
                        diff = c1.cln_db_path.CompareTo(c2.cln_db_path);
                    return diff;
                });
            }

            return result;
        }

        public List<UpdateCountModel> GetUpdatesForPaging(long idClient, long idProduct, int page, out int totalPages, int pageSize = 25, bool asc = true)
        {
            totalPages = 1;
            if (pageSize < 25) pageSize = 25;

            List<UpdateCountModel> results = new List<UpdateCountModel>();
            ClientModel client = GetClient(idClient);
            ProductModel product = GetProduct(idProduct);
            if (client == null || product == null)
                return results;

            int total = 0;
            string sql = 
                "select count(*) as CNT \r\n" +
                "  from update_count \r\n" +
                " where upd_client_id = " + idClient + " \r\n" +
                "   and upd_product_id = " + idProduct + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        string stmp = rdr["CNT"] is System.DBNull ? "0" : rdr["CNT"].ToString();
                        if (!int.TryParse(stmp, out total) || total < 0) total = 0;
                    }
                }
            }

            decimal tmp = total / pageSize;
            totalPages = (int)Math.Round(tmp, 0, MidpointRounding.AwayFromZero);
            if (totalPages > 0)
                if (total % totalPages > 0) totalPages++;

            string dir = asc ? " ASC" : " DESC";
            sql =
                "select *, \r\n" +
                "       (select sum(updunit_count) from update_unit where updunit_upd_id = upd_id) as upd_count \r\n" +
                "  from update_count \r\n" +
                " where upd_client_id = " + idClient + " \r\n" +
                "   and upd_product_id = " + idProduct + " \r\n" +
                " order by \r\n" +
                "       upd_date " + dir + "\r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while(rdr.Read())
                    {
                        UpdateCountModel update = new UpdateCountModel();
                        update.upd_id = (long)rdr["upd_id"];
                        update.upd_date = LongToDate((long)rdr["upd_date"]);
                        update.upd_client_id = (long)rdr["upd_client_id"];
                        update.upd_product_id = (long)rdr["upd_product_id"];
                        update.upd_count = rdr["upd_count"] is System.DBNull ? 0 : (long)rdr["upd_count"];
                        update.Client = client;
                        update.Product = product;
                        results.Add(update);
                    }
                }
            }

            return results;
        }

        public List<UpdateDetailsModel> GetUpdateDetails(DateTime dtStart, DateTime dtEnd)
        {
            List<UpdateDetailsModel> result = new List<UpdateDetailsModel>();
            long start = DateToLong(dtStart);
            long end = DateToLong(dtEnd);
            string sql =
                "select unit_name,  \r\n" +
                "       updunit_count,  \r\n" +
                "       upd_date, \r\n" +
                "       prod_name,  \r\n" +
                "       cln_name, \r\n" +
                "       cln_db_path, \r\n" +
                "       account_name \r\n" +
                "  from update_unit \r\n" +
                "  LEFT OUTER JOIN unit ON updunit_unit_id = unit_id \r\n" +
                "  LEFT OUTER JOIN update_count ON updunit_upd_id = upd_id \r\n" +
                "  LEFT OUTER JOIN product ON upd_product_id = prod_id \r\n" +
                "  LEFT OUTER JOIN client ON upd_client_id = cln_id \r\n" +
                "  LEFT OUTER JOIN account ON cln_account_id = account_id \r\n" +
               $" WHERE upd_date between {start} and {end} \r\n" +
                " order by \r\n" +
                "       upd_date DESC, \r\n" +
                "       account_name, \r\n" +
                "       cln_name, \r\n" +
                "       cln_db_path, \r\n" +
                "       prod_name, \r\n" +
                "       unit_name \r\n" +
                "";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        UpdateDetailsModel detail = new UpdateDetailsModel();
                        detail.Unit = rdr["unit_name"].ToString();
                        detail.Count = (long)rdr["updunit_count"];
                        detail.Date = LongToDate((long)rdr["upd_date"]).ToString("yyyy-MM-dd HH:mm:ss");
                        detail.Product = rdr["prod_name"].ToString();
                        detail.Client = rdr["cln_name"].ToString();
                        detail.Database = rdr["cln_db_path"].ToString();
                        detail.Account = rdr["account_name"].ToString();
                        result.Add(detail);
                    }
                }
            }

            return result;
        }

        public List<List<string>> GetUpdateDetailsForJson(DateTime dtStart, DateTime dtEnd)
        {
            List<List<string>> results = new List<List<string>>();
            long start = DateToLong(dtStart);
            long end = DateToLong(dtEnd);
            string sql =
                    "select upd_date,  \r\n" +
                    "       account_number,  \r\n" +
                    "       account_name,  \r\n" +
                    "       cln_name, \r\n" +
                    "       cln_db_path,  \r\n" +
                    "       prod_name, \r\n" +
                    "       unit_name, \r\n" +
                    "       updunit_count \r\n" +
                    "  from update_unit \r\n" +
                    "  LEFT OUTER JOIN unit ON updunit_unit_id = unit_id \r\n" +
                    "  LEFT OUTER JOIN update_count ON updunit_upd_id = upd_id \r\n" +
                    "  LEFT OUTER JOIN product ON upd_product_id = prod_id \r\n" +
                    "  LEFT OUTER JOIN client ON upd_client_id = cln_id \r\n" +
                    "  LEFT OUTER JOIN account ON cln_account_id = account_id \r\n" +
                   $" WHERE upd_date between {start} and {end} \r\n" +
                    " order by \r\n" +
                    "       upd_date DESC, \r\n" +
                    "       account_number, \r\n" +
                    "       cln_name, \r\n" +
                    "       cln_db_path, \r\n" +
                    "       prod_name, \r\n" +
                    "       unit_name \r\n" +
                    "";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (var rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        DateTime dt = LongToDate((long)rdr["upd_date"]);
                        List<string> rec = new List<string> {
                            dt.ToString("yyyy-MM-dd"),
                            dt.ToString("HH:mm:ss"),
                            rdr["account_number"].ToString(),
                            rdr["account_name"].ToString(),
                            rdr["cln_name"].ToString(),
                            rdr["cln_db_path"].ToString(),
                            rdr["prod_name"].ToString(),
                            rdr["unit_name"].ToString(),
                            rdr["updunit_count"].ToString()
                        };
                        results.Add(rec);
                    }
                }
            }

            return results;
        }

        #endregion Client

        #region Helper Functions

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

        #endregion Helper Functions

        #region IDispose

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

        #endregion IDispose
    }
}