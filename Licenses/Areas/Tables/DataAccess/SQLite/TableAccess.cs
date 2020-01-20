using Licenses.Areas.Tables.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.DataAccess.SQLite
{
    public class TableAccess : ITableAccess
    {
        SQLiteConnection _access = null;
        DateTime _dateOffset = DateTime.Parse("01/01/1970 00:00:00");

        public TableAccess()
        {
            string file = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/"), "licenses.db");
            if (!File.Exists(file))
                throw new ApplicationException($"The database file '{file}' was not found");

            string connect = $"Data Source={file};Version=3;";
            _access = new SQLiteConnection(connect);
            _access.Open();
        }

        #region DataAccess Helper Functions

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

        #endregion DataAccess Helper Functions

        #region Account Methods

        public int AddAccount(AccountModel account)
        {
            if (account == null) throw new ArgumentNullException("Empty account passed to AddAccount");
            if (string.IsNullOrWhiteSpace(account.account_name)) throw new ArgumentNullException("account with empty acccount_name passed to AddAccount");

            string sql =
                "insert into account \r\n" +
                "( \r\n" +
                "   account_name, \r\n" +
                "   account_number \r\n" +
                ") \r\n" +
                "values \r\n" +
                "( \r\n" +
                "   @name, \r\n" +
                "   " + account.account_number + " \r\n" +
                ") \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", account.account_name ?? "");
                return ExecuteNonQuery(cmd);
            }
        }

        public int DeleteAccount(long account_id)
        {
            string sql = 
                "update client \r\n" +
                "   set cln_account_id = null \r\n" +
                " where cln_account_id = " + account_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                int cnt = ExecuteNonQuery(cmd);
            }

            sql =
                "delete from account \r\n" +
                " where account_id = " + account_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        public AccountModel GetAccount(long account_id)
        {
            AccountModel result = null;
            string sql =
                "select * \r\n" +
                "  from account \r\n" +
                " where account_id = " + account_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new AccountModel(rdr);
                    }
                }
            }

            return result;
        }

        public long GetAccountId(long account_number)
        {
            string sql =
                "select account_id \r\n" +
                "  from account \r\n" +
                " where account_number = " + account_number + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        return (long)rdr["account_id"];
                    }
                }
            }

            return -1;
        }

        public long GetAccountId(string name)
        {
            string sql =
                "select account_id \r\n" +
                "  from account \r\n" +
                " where account_name = @name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", name ?? "");
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        return (long)rdr["account_id"];
                    }
                }
            }

            return -1;
        }

        public List<AccountModel> GetAccounts()
        {
            List<AccountModel> result = new List<AccountModel>();
            string sql =
                "select * \r\n" +
                "  from account \r\n" +
                " order by account_name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        result.Add(new AccountModel(rdr));
                    }
                }
            }

            return result;
        }

        public int UpdateAccount(long account_id, FormCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("Empty collection passed to UpdateAccount");
            string account_name = (string)collection.GetValue("account_name").ConvertTo(typeof(string));
            long account_number = (long)collection.GetValue("account_number").ConvertTo(typeof(long));
            string sql =
                "update account \r\n" +
                "   set account_number = " + account_number + ", \r\n" +
                "       account_name = @name \r\n" +
                " where account_id = " + account_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", account_name ?? "");
                return ExecuteNonQuery(cmd);
            }
        }

        #endregion Account Methods

        #region Client Methods

        public int AddClient(ClientModel client)
        {
            if (client == null) throw new ArgumentNullException("Empty client passed to AddClient");
            if (string.IsNullOrWhiteSpace(client.cln_name)) throw new ArgumentNullException("client with empty cln_name passed to AddClient");
            if (string.IsNullOrWhiteSpace(client.cln_db_path)) throw new ArgumentNullException("client with empty cln_db_path passed to AddClient");
            string sql =
                "insert into client \r\n" +
                "( \r\n" +
                "   cln_name, \r\n" +
                "   cln_db_path, \r\n" +
                "   cln_account_id \r\n" +
                ") \r\n" +
                "values \r\n" +
                "( \r\n" +
                "   @name, \r\n" +
                "   @dbPath, \r\n" +
                "   " + (client.cln_account_id.HasValue ? client.cln_account_id.ToString() : "null") + " \r\n" +
                ") \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", client.cln_name ?? "");
                cmd.Parameters.AddWithValue("@dbPath", client.cln_db_path ?? "");
                return ExecuteNonQuery(cmd);
            }
        }

        public int DeleteClient(long cln_id)
        {
            string sql =
                "delete from client \r\n" +
                " where cln_id = " + cln_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        public ClientModel GetClient(long cln_id, bool fillAvailableLists = true)
        {
            ClientModel result = null;
            string sql =
                "select client.*, account_number \r\n" +
                "  from client \r\n" +
                "  LEFT OUTER JOIN account ON cln_account_id = account_id \r\n" +
                " where cln_id = " + cln_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new ClientModel(rdr);
                        result.AccountNumber = rdr["account_number"] is DBNull ? "null" : rdr["account_number"].ToString();
                        if (fillAvailableLists)
                            result.AvailableAccounts = GetAvailableAccounts();
                    }
                }
            }

            return result;
        }

        public long GetClientId(string name, string database)
        {
            string sql =
                "select cln_id \r\n" +
                "  from client \r\n" +
                " where cln_name = @name \r\n" + 
                "   and cln_db_path = @dbPath \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", name ?? "");
                cmd.Parameters.AddWithValue("@dbPath", database ?? "");
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        return (long)rdr["cln_id"];
                    }
                }
            }

            return -1;
        }

        public List<ClientModel> GetClients()
        {
            List<ClientModel> result = new List<ClientModel>();
            string sql =
                "select client.*, account_number \r\n" +
                "  from client \r\n" +
                "  LEFT OUTER JOIN account ON cln_account_id = account_id \r\n" +
                " order by \r\n" +
                "       cln_name, \r\n" +
                "       cln_db_path \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        ClientModel model = new ClientModel(rdr);
                        model.AccountNumber = rdr["account_number"] is DBNull ? "none" : rdr["account_number"].ToString();
                        result.Add(model);
                    }
                }
            }

            return result;
        }

        public int UpdateClient(long cln_id, FormCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("Empty collection passed to UpdateClient");
            string cln_name = (string)collection.GetValue("cln_name").ConvertTo(typeof(string));
            string cln_db_path = (string)collection.GetValue("cln_db_path").ConvertTo(typeof(string));
            long? cln_account_id = null;
            foreach (string key in collection.AllKeys)
            {
                if (key == "cln_account_id")
                {
                    cln_account_id = (long?)collection.GetValue("cln_account_id").ConvertTo(typeof(long?));
                    if (cln_account_id < 0)
                        cln_account_id = null;
                    break;
                }
            }

            string sql =
                "update client \r\n" +
                "   set cln_name = @name, \r\n" +
                "       cln_db_path = @dbPath, \r\n" +
                "       cln_account_id = " + (cln_account_id.HasValue ? cln_account_id.Value.ToString() : "null") + " \r\n" +
                " where cln_id = " + cln_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", cln_name ?? "");
                cmd.Parameters.AddWithValue("@dbPath", cln_db_path ?? "");
                return ExecuteNonQuery(cmd);
            }
        }

        #endregion Client Methods

        #region Product Methods

        public int AddProduct(ProductModel product)
        {
            if (product == null) throw new ArgumentNullException("Empty product passed to AddProduct");
            if (string.IsNullOrWhiteSpace(product.prod_name)) throw new ArgumentNullException("product with empty prod_name passed to AddProduct");

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
                cmd.Parameters.AddWithValue("@name", product.prod_name ?? "");
                return ExecuteNonQuery(cmd);
            }
        }

        public int DeleteProduct(long prod_id)
        {
            string sql =
                "delete from product \r\n" +
                " where prod_id = " + prod_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        public ProductModel GetProduct(long prod_id)
        {
            ProductModel result = null;
            string sql =
                "select * \r\n" +
                "  from product \r\n" +
                " where prod_id = " + prod_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new ProductModel(rdr);
                    }
                }
            }

            return result;
        }

        public long GetProductId(string name)
        {
            string sql =
                "select prod_id \r\n" +
                "  from product \r\n" +
                " where prod_name = @name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", name ?? "");
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        return (long)rdr["prod_id"];
                    }
                }
            }

            return -1;
        }

        public List<ProductModel> GetProducts()
        {
            List<ProductModel> result = new List<ProductModel>();
            string sql =
                "select * \r\n" +
                "  from product \r\n" +
                " order by prod_name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        result.Add(new ProductModel(rdr));
                    }
                }
            }

            return result;
        }

        public int UpdateProduct(long prod_id, FormCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("Empty collection passed to UpdateProduct");
            string prod_name = (string)collection.GetValue("prod_name").ConvertTo(typeof(string));
            string sql =
                "update product \r\n" +
                "   set prod_name = @name \r\n" +
                " where prod_id = " + prod_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", prod_name ?? "");
                return ExecuteNonQuery(cmd);
            }
        }

        #endregion Product Methods

        #region Unit Methods

        public int AddUnit(UnitModel unit)
        {
            if (unit == null) throw new ArgumentNullException("Empty unit passed to AddUnit");
            if (string.IsNullOrWhiteSpace(unit.unit_name)) throw new ArgumentNullException("unit with empty unit_name passed to AddUnit");

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
                cmd.Parameters.AddWithValue("@name", unit.unit_name ?? "");
                return ExecuteNonQuery(cmd);
            }
        }

        public int DeleteUnit(long unit_id)
        {
            string sql =
                "delete from unit \r\n" +
                " where unit_id = " + unit_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        public UnitModel GetUnit(long unit_id)
        {
            UnitModel result = null;
            string sql =
                "select * \r\n" +
                "  from unit \r\n" +
                " where unit_id = " + unit_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new UnitModel(rdr);
                    }
                }
            }

            return result;
        }

        public long GetUnitId(string name)
        {
            string sql =
                "select unit_id \r\n" +
                "  from unit \r\n" +
                " where unit_name = @name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", name ?? "");
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        return (long)rdr["unit_id"];
                    }
                }
            }

            return -1;
        }

        public List<UnitModel> GetUnits()
        {
            List<UnitModel> result = new List<UnitModel>();
            string sql =
                "select * \r\n" +
                "  from unit \r\n" +
                " order by unit_name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        result.Add(new UnitModel(rdr));
                    }
                }
            }

            return result;
        }

        public int UpdateUnit(long unit_id, FormCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("Empty collection passed to UpdateUnit");
            string unit_name = (string)collection.GetValue("unit_name").ConvertTo(typeof(string));
            string sql =
                "update unit \r\n" +
                "   set unit_name = @name \r\n" +
                " where unit_id = " + unit_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                cmd.Parameters.AddWithValue("@name", unit_name ?? "");
                return ExecuteNonQuery(cmd);
            }
        }

        #endregion Unit Methods

        #region Update Count Methods

        public int AddUpdateCount(UpdateCountModel updateCount)
        {
            if (updateCount == null) throw new ArgumentNullException("Empty updateCount passed to AddUpdateCount");
          //if (updateCount.upd_count < 0) throw new ArgumentNullException("updateCount with invalid upd_count (" + updateCount.upd_count + ") passed to AddUpdateCount");
            long upd_date = DateToLong(updateCount.UpdateDate);

            string sql =
                "insert into update_count \r\n" +
                "( \r\n" +
                "   upd_date, \r\n" +
                "   upd_client_id, \r\n" +
                "   upd_product_id \r\n" +
                ") \r\n" +
                "values \r\n" +
                "( \r\n" +
                "   " + upd_date + ", \r\n" +
                "   " + updateCount.upd_client_id + ", \r\n" +
                "   " + updateCount.upd_product_id + " \r\n" +
                ") \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        public int DeleteUpdateCount(long upd_id)
        {
            string sql =
                "delete from update_count \r\n" +
                " where upd_id = " + upd_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        public UpdateCountModel GetUpdateCount(long upd_id, bool fillAvailableLists = true)
        {
            UpdateCountModel result = null;
            string sql =
                "select u.*, \r\n" + 
                "       c.cln_name, \r\n" + 
                "       c.cln_db_path, " +
                "       p.prod_name, \r\n" +
                "       (select sum(updunit_count) from update_unit where updunit_upd_id = upd_id) as upd_count \r\n" +
                "  from update_count u \r\n" +
                "  LEFT OUTER JOIN client c ON u.upd_client_id = c.cln_id \r\n" +
                "  LEFT OUTER JOIN product p ON u.upd_product_id = p.prod_id \r\n" +
                " where u.upd_id = " + upd_id + " \r\n" +
                " order by upd_date desc \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new UpdateCountModel(rdr);
                        result.UpdateDate = LongToDate(result.upd_date);
                        result.ClientName = (rdr["cln_name"] is System.DBNull ? "" : rdr["cln_name"].ToString()) + " | " +
                                            (rdr["cln_db_path"] is System.DBNull ? "" : rdr["cln_db_path"].ToString());
                        result.ProductName = rdr["prod_name"] is System.DBNull ? "" : rdr["prod_name"].ToString();
                        if (fillAvailableLists)
                        {
                            result.AvailableClients = GetAvailableClients();
                            result.AvailableProducts = GetAvailableProducts();
                        }
                    }
                }
            }

            return result;
        }

        public List<UpdateCountModel> GetUpdateCounts(DateTime dtStart, DateTime dtEnd, long idClient = 0, long idProduct = 0)
        {
            List<UpdateCountModel> result = new List<UpdateCountModel>();
            long start = DateToLong(dtStart);
            long end = DateToLong(dtEnd);
            string sql =
                "select u.*, \r\n" + 
                "       c.cln_name, \r\n" + 
                "       c.cln_db_path, \r\n" +
                "       p.prod_name, \r\n" +
                "       (select sum(updunit_count) from update_unit where updunit_upd_id = upd_id) as upd_count \r\n" +
                "  from update_count u \r\n" +
                "  LEFT OUTER JOIN client c ON u.upd_client_id = c.cln_id \r\n" +
                "  LEFT OUTER JOIN product p ON u.upd_product_id = p.prod_id \r\n" +
               $" where upd_date >= {start} and upd_date <= {end} \r\n" +
               (idClient > 0 ? $"   and upd_client_id = {idClient} \r\n" : "") +
               (idProduct > 0 ? $"   and upd_product_id = {idProduct} \r\n" : "") +
                " order by upd_date desc \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        UpdateCountModel count = new UpdateCountModel(rdr);
                        count.ClientName = (rdr["cln_name"] is System.DBNull ? "" : rdr["cln_name"].ToString()) + 
                                           " | " +
                                           (rdr["cln_db_path"] is System.DBNull ? "" : rdr["cln_db_path"].ToString());
                        count.ProductName = rdr["prod_name"] is System.DBNull ? "" : rdr["prod_name"].ToString();
                        count.UpdateDate = LongToDate(count.upd_date);
                        result.Add(count);
                    }
                }
            }

            return result;
        }

        public List<UpdateCountModel> GetUpdatesCountsForPaging(long idClient, long idProduct, int page, out int totalPages, int pageSize = 10, bool asc = true)
        {
            totalPages = 1;
            if (pageSize < 10) pageSize = 10;

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
                    while (rdr.Read())
                    {
                        UpdateCountModel update = new UpdateCountModel();
                        update.upd_id = (long)rdr["upd_id"];
                        update.UpdateDate = LongToDate((long)rdr["upd_date"]);
                        update.upd_client_id = (long)rdr["upd_client_id"];
                        update.upd_product_id = (long)rdr["upd_product_id"];
                        update.upd_count = rdr["upd_count"] is System.DBNull ? 0 : (long)rdr["upd_count"];
                        update.ClientName = rdr["cln_name"] is System.DBNull ? "" : rdr["cln_name"].ToString();
                        update.ProductName = rdr["prod_name"] is System.DBNull ? "" : rdr["prod_name"].ToString();
                        update.UpdateDate = LongToDate(update.upd_date);
                        results.Add(update);
                    }
                }
            }

            return results;
        }

        public int UpdateUpdateCount(long upd_id, FormCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("Empty collection passed to UpdateProduct");
            DateTime date = (DateTime)collection.GetValue("UpdateDate").ConvertTo(typeof(DateTime));
            long upd_date = DateToLong(date);
            long upd_client_id = (long)collection.GetValue("upd_client_id").ConvertTo(typeof(long));
            long upd_product_id = (long)collection.GetValue("upd_product_id").ConvertTo(typeof(long));
          //long upd_count = (long)collection.GetValue("upd_count").ConvertTo(typeof(long));
            string sql =
                "update update_count \r\n" +
                "   set upd_date = " + upd_date + ", \r\n" +
                "       upd_client_id = " + upd_client_id + ", \r\n" +
                "       upd_product_id = " + upd_product_id + " \r\n" +
              //"       upd_count = " + upd_count + " \r\n" +
                " where upd_id = " + upd_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        #endregion Update Count Methods

        #region Update Units Methods

        public int AddUpdateUnit(UpdateUnitModel updateUnit)
        {
            if (updateUnit == null) throw new ArgumentNullException("Empty updateUnit passed to AddUpdateUnit");
            if (updateUnit.updunit_count < 0) throw new ArgumentNullException($"updateUnit with invalid updunit_count ({updateUnit.updunit_count}) passed to AddUpdateUnit");

            string sql =
                "insert into update_unit \r\n" +
                "( \r\n" +
                "   updunit_upd_id, \r\n" +
                "   updunit_unit_id, \r\n" +
                "   updunit_count \r\n" +
                ") \r\n" +
                "values \r\n" +
                "( \r\n" +
                "   " + updateUnit.updunit_upd_id + ", \r\n" +
                "   " + updateUnit.updunit_unit_id + ", \r\n" +
                "   " + updateUnit.updunit_count + " \r\n" +
                ") \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        public int DeleteUpdateUnit(long updunit_id)
        {
            string sql =
                "delete from update_unit \r\n" +
                " where updunit_id = " + updunit_id + " \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        public UpdateCountModel GetUpdateCountWithUnits(long upd_id)
        {
            UpdateCountModel result = GetUpdateCount(upd_id, false);
            if (result == null)
                return result;
            List<SelectListItem> units = GetAvailableUnits();
            string sql =
                "select uu.*, \r\n" +
                "	    u.unit_name \r\n" +
                "  from update_unit uu \r\n" +
                "  LEFT OUTER JOIN unit u ON uu.updunit_unit_id = u.unit_id \r\n" +
               $" where uu.updunit_upd_id = {upd_id} \r\n" +
                " order by \r\n " + 
                "       unit_name \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        UpdateUnitModel uu = new UpdateUnitModel(rdr);
                        uu.UnitName = rdr["unit_name"] is System.DBNull ? "" : rdr["unit_name"].ToString();
                        uu.AvailableUnits = units;
                        result.Units.Add(uu);
                    }
                }
            }

            return result;
        }

        public UpdateUnitModel GetUpdateUnit(long updunit_id)
        {
            UpdateUnitModel result = null;
            string sql =
                "select uu.*, \r\n" +
                "	    u.unit_name \r\n" +
                "  from update_unit uu \r\n" +
                "  LEFT OUTER JOIN unit u ON uu.updunit_unit_id = u.unit_id \r\n" +
               $" where updunit_id = {updunit_id} \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                using (SQLiteDataReader rdr = ExecuteReader(cmd))
                {
                    if (rdr.Read())
                    {
                        result = new UpdateUnitModel(rdr);
                        result.UnitName = rdr["unit_name"] is System.DBNull ? "" : rdr["unit_name"].ToString();
                    }
                }
            }

            return result;
        }

        public int UpdateUpdateUnit(long updunit_id, FormCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("Empty collection passed to UpdateUpdateUnit");
            long updunit_unit_id = (long)collection.GetValue("updunit_unit_id").ConvertTo(typeof(long));
            long updunit_count = (long)collection.GetValue("updunit_count").ConvertTo(typeof(long));

            if (updunit_count < 0) throw new ArgumentNullException($"updateUnit with invalid updunit_count ({updunit_count}) passed to UpdateUpdateUnit");

            string sql =
                "update update_unit \r\n" +
               $"   set updunit_unit_id = {updunit_unit_id}, \r\n" +
               $"       updunit_count = {updunit_count} \r\n" +
               $" where updunit_id = {updunit_id} \r\n";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, _access))
            {
                return ExecuteNonQuery(cmd);
            }
        }

        #endregion Update Units Methods

        #region Helper Methods

        public List<SelectListItem> GetAvailableAccounts()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem { Text = "", Value = "" });

            List<AccountModel> accounts = GetAccounts();
            foreach (var account in accounts)
            {
                result.Add(new SelectListItem { Text = account.account_number + " - " + account.account_name, Value = account.account_id.ToString() });
            }

            return result;
        }

        public List<SelectListItem> GetAvailableClients(bool allowEmpty = false)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            if (allowEmpty)
                result.Add(new SelectListItem { Text = " ", Value = "0" });

            List<ClientModel> clients = GetClients();
            foreach (var client in clients)
            {
                result.Add(new SelectListItem { Text = client.cln_name + " | " + client.cln_db_path, Value = client.cln_id.ToString() });
            }

            return result;
        }

        public List<SelectListItem> GetAvailableProducts(bool allowEmpty = false)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            if (allowEmpty)
                result.Add(new SelectListItem { Text = " ", Value = "0" });

            List<ProductModel> products = GetProducts();
            foreach (var prod in products)
            {
                result.Add(new SelectListItem { Text = prod.prod_name, Value = prod.prod_id.ToString() });
            }

            return result;
        }

        public List<SelectListItem> GetAvailableUnits()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            List<UnitModel> products = GetUnits();
            foreach (var unit in products)
            {
                result.Add(new SelectListItem { Text = unit.unit_name, Value = unit.unit_id.ToString() });
            }

            return result;
        }

        #endregion Helper Methods

        #region IDisposable

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

        #endregion IDisposable
    }
}
