using Licenses.Library.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYLICCLN
{
    class License : IDisposable
    {
        public const string EXECUTRAK = "EXECUTRAK";
        public const string DEALERTRAK = "DEALERTRAK";
        public const string DELIVERYTRAK = "DELIVERYTRAK";
        public const string FUELTRAK = "FUELTRAK";
        public const string STORETRAK = "STORETRAK";

        OdbcConnection _con = null;

        public License(string connection)
        {
            _con = new OdbcConnection(Program.Connection);
            _con.Open();

            GetUserName();
            GetClientName();
            CheckSent();
        }

        public bool ExecuTrakSent { get; private set; } = false;

        public bool DealerTrakSent { get; private set; } = false;

        public bool DeliveryTrakSent { get; private set; } = false;

        public bool FuelTrakSent { get; private set; } = false;

        public bool StoreTrakSent { get; private set; } = false;

        public bool AllSent { get; private set; } = false;

        public string UserName { get; private set; } = "Blank User";

        public string ClientName { get; private set; } = "Blank Client";

        public string LicenseUrl { get; private set; } = "https://localhost:44312/api/LicenseCount";

        public void DoUpdates()
        {
            if (!ExecuTrakSent)
            {
                List<UpdateUnitDTO> units = GetExecuTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "ExecuTrak", DateTime.Now, units);
                //ResultDTO result = Library.Server.Tools.PostToServer(dto, url, false);
            }

            if (!FuelTrakSent)
            {
                List<UpdateUnitDTO> units = GetFuelTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "FuelTrak", DateTime.Now, units);
            }

            if (!DeliveryTrakSent)
            {
                List<UpdateUnitDTO> units = GetDeliveryTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "DeliveryTrak", DateTime.Now, units);
            }

            if (!StoreTrakSent)
            {
                List<UpdateUnitDTO> units = GetStoreTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "StoreTrak", DateTime.Now, units);
            }

            if (!DealerTrakSent)
            {
                List<UpdateUnitDTO> units = GetDealerTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "DealerTrak", DateTime.Now, units);
            }
        }

        void CheckSent()
        {
            string sql = "";
            try
            {
                string today = DateTime.Today.ToShortDateString();
                sql =
                    "select ini_field_name, \r\n" +
                    "       ini_value \r\n" +
                    "  from sys_ini \r\n" +
                    " where ini_file_name = 'SYLICCLN' \r\n" +
                    "   and ini_section = 'UPDATE' \r\n";
                using (var rdr = ExecuteReader(sql))
                {
                    while (rdr.Read())
                    {
                        string name = rdr["ini_field_name"] is DBNull ? "" : rdr["ini_field_name"].ToString().Trim().ToUpper();
                        string date = rdr["ini_value"] is DBNull ? "" : rdr["ini_value"].ToString().Trim().ToUpper();

                        DateTime dt;
                        if (DateTime.TryParse(date, out dt))
                        {
                            bool sent = dt.ToShortDateString() == today;
                            if (sent)
                            {
                                switch (name)
                                {
                                    case EXECUTRAK: ExecuTrakSent = true; break;
                                    case DEALERTRAK: DealerTrakSent = true; break;
                                    case DELIVERYTRAK: ExecuTrakSent = true; break;
                                    case FUELTRAK: FuelTrakSent = true; break;
                                    case STORETRAK: StoreTrakSent = true; break;
                                }
                            }
                        }
                    }
                }

                AllSent = ExecuTrakSent && DealerTrakSent && ExecuTrakSent && FuelTrakSent && StoreTrakSent;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("CheckSent error: " + ex.Message + "\r\n" + sql, ex);
            }
        }

        void GetUserName()
        {
            if (string.IsNullOrWhiteSpace(Program.Connection))
                return;

            // User Id, UID
            string ucon = Program.Connection;
            string[] parts = ucon.Split(';');
            foreach(string part in parts)
            {
                int eq = part.IndexOf('=');
                if (eq > 0)
                {
                    string name = part.Substring(0, eq).Trim().ToUpper();
                    string value = part.Substring(eq + 1).Trim();
                    if (name == "USER ID" || name == "UID")
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            UserName = value;
                            break;
                        }
                    }
                }
            }
        }

        void GetClientName()
        {
            string sql = "";
            try
            {
                sql =
                    "select con_name \r\n" +
                    "  from co_company_name \r\n";
                using (var rdr = ExecuteReader(sql))
                {
                    if (rdr.Read())
                    {
                        string name = rdr["con_name"] is DBNull ? "" : rdr["con_name"].ToString().Trim().ToUpper();
                        if (!string.IsNullOrWhiteSpace(name))
                            this.ClientName = name;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("GetClientName error: " + ex.Message + "\r\n" + sql, ex);
            }
        }

        void GetLicenseUrl()
        {
            string sql = "";
            try
            {

            }
            catch (Exception ex)
            {
                throw new ApplicationException("GetLicenseUrl error: " + ex.Message + "\r\n" + sql, ex);
            }
        }

        public int FlagExecuTrakUser()
        {
            string sql = "";
            try
            {
                if (string.IsNullOrWhiteSpace(UserName))
                    return -1;

                string date = DateTime.Today.ToString("yyyy-MM-dd");
                string user = UserName.Replace("'", "''");
                sql =
                    "select ini_value \r\n" +
                    "  from sys_ini \r\n" +
                    " where ini_file_name = 'SYLICCLN' \r\n" +
                    "   and ini_user_id = 'EXECTRAK' \r\n" +
                    "   and ini_section = 'FLAG' \r\n" +
                   $"   and ini_field_name = '{user}' \r\n";

                bool update = false;
                using (var rdr = ExecuteReader(sql))
                {
                    if (rdr.Read())
                    {
                        update = true;

                        // already done?
                        string tmp = rdr["ini_value"] is DBNull ? "" : rdr["ini_value"].ToString().Trim();
                        if (tmp == date)
                            return 0;
                    }
                }

                if (update)
                {
                    sql =
                        "update sys_ini \r\n" +
                       $"   set ini_value = '{date}' \r\n" +
                        " where ini_file_name = 'SYLICCLN' \r\n" +
                        "   and ini_user_id = 'EXECTRAK' \r\n" +
                        "   and ini_section = 'FLAG' \r\n" +
                       $"   and ini_field_name = '{user}' \r\n";
                }
                else
                {
                    sql =
                        "insert into sys_ini \r\n" +
                        "( \r\n" +
                        "  ini_file_name, \r\n" +
                        "  ini_user_id, \r\n" +
                        "  ini_section, \r\n" +
                        "  ini_field_name, \r\n" +
                        "  ini_value \r\n" +
                        ") \r\n" +
                        "values \r\n" +
                        "( \r\n" +
                        "  'SYLICCLN', \r\n" +
                       $"  'EXECTRAK', \r\n" +
                        "  'FLAG', \r\n" +
                       $"  '{user}', \r\n" +
                       $"  '{date}' \r\n" +
                       ") \r\n";
                }

                int cnt = ExecuteNonQuery(sql);
                return cnt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error in FlagExecuTrakUser: {ex.Message}: \r\n{sql}", ex);
            }
        }

        List<UpdateUnitDTO> GetExecuTrakUnits()
        {
            string sql = "";
            try
            {
                List<UpdateUnitDTO> result = new List<UpdateUnitDTO>();
                string cutoff = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
                sql =
                    "select distinct ini_field_name \r\n" +
                    "  from sys_ini \r\n" +
                    " where ini_file_name = 'SYLICCLN' \r\n" +
                    "   and ini_user_id = 'EXECTRAK' \r\n" +
                    "   and ini_section = 'FLAG' \r\n" +
                   $"   and ini_value >= '{cutoff}' \r\n";
                using (var rdr = ExecuteReader(sql))
                {
                    while (rdr.Read())
                    {
                        string name = rdr["ini_field_name"] is DBNull ? "<blank>" : rdr["ini_field_name"].ToString().Trim();
                        result.Add(new UpdateUnitDTO { UnitName = "User " + name, UnitCount = 1 });
                    }
                }

                if (result.Count == 0)
                    result.Add(new UpdateUnitDTO { UnitName = "Users", UnitCount = 0 });

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error in GetExecuTrakUnits: {ex.Message}: \r\n{sql}", ex);
            }
        }

        List<UpdateUnitDTO> GetFuelTrakUnits()
        {
            string sql = "";
            try
            {
                List<UpdateUnitDTO> result = new List<UpdateUnitDTO>();
                string cutoff = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
                sql =
                    "select distinct sct_truck \r\n" +
                    "  from sc_truck \r\n" +
                    " where sct_active = 'Y' \r\n" +
                    "   and sct_scd = 'FO' \r\n" +
                    "   and sct_pc_onboard = 1 \r\n";
                using (var rdr = ExecuteReader(sql))
                {
                    while (rdr.Read())
                    {
                        string name = rdr["sct_truck"] is DBNull ? "<blank>" : rdr["sct_truck"].ToString().Trim();
                        result.Add(new UpdateUnitDTO { UnitName = "Truck " + name, UnitCount = 1 });
                    }
                }

                if (result.Count == 0)
                    result.Add(new UpdateUnitDTO { UnitName = "Trucks", UnitCount = 0 });

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error in GetFuelTrakUnits: {ex.Message}: \r\n{sql}", ex);
            }
        }

        List<UpdateUnitDTO> GetDeliveryTrakUnits()
        {
            string sql = "";
            try
            {
                List<UpdateUnitDTO> result = new List<UpdateUnitDTO>();
                string dtFrom = DateTime.Today.AddDays(-30).ToString("MM/dd/yyyy");
                string dtTo = DateTime.Today.ToString("MM/dd/yyyy");
                sql =
                    "select distinct fddm_truck \r\n" +
                    "  from fd_dispatch_mtr \r\n" +
                   $" where fddm_dispatch_dt between '{dtFrom}' and '{dtTo}' \r\n" +
                    " prder by 1";
                using (var rdr = ExecuteReader(sql))
                {
                    while (rdr.Read())
                    {
                        int truck = rdr["sct_truck"] is DBNull ? -1 : (int)rdr["sct_truck"];
                        result.Add(new UpdateUnitDTO { UnitName = "Truck " + truck.ToString(), UnitCount = 1 });
                    }
                }

                if (result.Count == 0)
                    result.Add(new UpdateUnitDTO { UnitName = "Trucks", UnitCount = 0 });

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error in GetDeliveryTrakTrakUnits: {ex.Message}: \r\n{sql}", ex);
            }
        }

        List<UpdateUnitDTO> GetStoreTrakUnits()
        {
            List<UpdateUnitDTO> result = new List<UpdateUnitDTO>();

            if (result.Count == 0)
                result.Add(new UpdateUnitDTO { UnitName = "Stores", UnitCount = 0 });

            return result;
        }

        List<UpdateUnitDTO> GetDealerTrakUnits()
        {
            List<UpdateUnitDTO> result = new List<UpdateUnitDTO>();

            if (result.Count == 0)
                result.Add(new UpdateUnitDTO { UnitName = "Stores", UnitCount = 0 });

            return result;
        }

        #region DataAccess

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            try
            {
                int cnt = 0;
                using (OdbcCommand cmd = new OdbcCommand(sql, _con))
                {
                    cnt = cmd.ExecuteNonQuery();
                }

                return cnt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error executing SQL: {ex.Message} \r\n{sql}\r\n", ex);
            }
        }

        /// <summary>
        /// WARNING: YOU are responsible for disposing of the reader object.
        /// It is HIGHLY suggested that you wrap this call in a using statement.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string sql)
        {
            try
            {
                using (OdbcCommand cmd = new OdbcCommand(sql, _con))
                {
                    OdbcDataReader rdr = cmd.ExecuteReader();
                    return rdr;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error executing SQL: {ex.Message} \r\n{sql}\r\n", ex);
            }
        }

        #endregion DataAccess

        #region IDisposable

        public void Dispose()
        {
            if (_con != null)
            {
                _con.Dispose();
                _con = null;
            }
        }

        #endregion IDisposable
    }
}
