using Licenses.Library.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
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

            GetArchiveFolder();
            GetUserName();
            GetClientName();
            GetLicenseUrl();
            GetSent();
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

        public string WorkingFolder { get; private set; }

        public string ArchiveFolder { get; private set; }

        public void DoUpdates()
        {
            DateTime dtSent = DateTime.Now;
            if (!ExecuTrakSent)
            {
                List<UpdateUnitDTO> units = GetExecuTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "ExecuTrak", dtSent, units);
                ResultDTO result = Licenses.Library.Server.Tools.PostToServer(dto, LicenseUrl, false);
                SetSent(EXECUTRAK, dtSent);
            }

            if (!FuelTrakSent)
            {
                List<UpdateUnitDTO> units = GetFuelTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "FuelTrak", dtSent, units);
                ResultDTO result = Licenses.Library.Server.Tools.PostToServer(dto, LicenseUrl, false);
                SetSent(FUELTRAK, dtSent);
            }

            if (!DeliveryTrakSent)
            {
                List<UpdateUnitDTO> units = GetDeliveryTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "DeliveryTrak", dtSent, units);
                ResultDTO result = Licenses.Library.Server.Tools.PostToServer(dto, LicenseUrl, false);
                SetSent(DELIVERYTRAK, dtSent);
            }

            if (!StoreTrakSent)
            {
                List<UpdateUnitDTO> units = GetStoreTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "StoreTrak", dtSent, units);
                ResultDTO result = Licenses.Library.Server.Tools.PostToServer(dto, LicenseUrl, false);
                SetSent(STORETRAK, dtSent);
            }

            if (!DealerTrakSent)
            {
                List<UpdateUnitDTO> units = GetDealerTrakUnits();
                UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, "DealerTrak", dtSent, units);
                ResultDTO result = Licenses.Library.Server.Tools.PostToServer(dto, LicenseUrl, false);
                SetSent(DEALERTRAK, dtSent);
            }
        }

        void GetSent()
        {
            string sql = "";
            try
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");
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

                        bool sent = date == today;
                        if (sent)
                        {
                            switch (name)
                            {
                                case EXECUTRAK: ExecuTrakSent = true; break;
                                case DEALERTRAK: DealerTrakSent = true; break;
                                case DELIVERYTRAK: DeliveryTrakSent = true; break;
                                case FUELTRAK: FuelTrakSent = true; break;
                                case STORETRAK: StoreTrakSent = true; break;
                            }
                        }
                    }
                }

                AllSent = ExecuTrakSent && DealerTrakSent && DeliveryTrakSent && FuelTrakSent && StoreTrakSent;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("CheckSent error: " + ex.Message + "\r\n" + sql, ex);
            }
        }

        void SetSent(string product, DateTime dtSent)
        {
            string sql = "";
            try
            {
                switch (product)
                {
                    case EXECUTRAK: break;
                    case DEALERTRAK: break;
                    case DELIVERYTRAK: break;
                    case FUELTRAK: break;
                    case STORETRAK: break;
                    default: return;
                }

                product = product.Replace("'", "''");

                sql =
                    "select max(ini_value) latest, count(*) cnt \r\n" +
                    "  from sys_ini \r\n" +
                    " where ini_file_name = 'SYLICCLN' \r\n" +
                    "   and ini_section = 'UPDATE' \r\n" +
                   $"   and ini_field_name = '{product}'";
                using (var rdr = ExecuteReader(sql))
                {
                    if (rdr.Read())
                    {
                        string tmp = rdr["cnt"] is DBNull ? "0" : rdr["cnt"].ToString();
                        int cnt;
                        if (!int.TryParse(tmp, out cnt))
                            cnt = 0;

                        if (cnt > 0)
                        {
                            DateTime dtLatest;
                            tmp = rdr["latest"] is DBNull ? "" : rdr["latest"].ToString().Trim();
                            if (!DateTime.TryParseExact(tmp, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtLatest))
                                dtLatest = DateTime.MinValue;

                            if (dtSent > dtLatest)
                            {
                                tmp = dtSent.ToString("yyyy-MM-dd");
                                sql =
                                    "update sys_ini \r\n" +
                                   $"   set ini_value = '{tmp}', \r\n" +
                                   $"       ini_user_id = '{UserName.Replace("'", "''")}' \r\n" +
                                    " where ini_file_name = 'SYLICCLN' \r\n" +
                                    "   and ini_section = 'UPDATE' \r\n" +
                                   $"   and ini_field_name = '{product}'";
                                ExecuteNonQuery(sql);
                            }
                        }
                        else
                        {
                            tmp = dtSent.ToString("yyyy-MM-dd");
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
                               $"  '{UserName ?? ""}', \r\n" +
                                "  'UPDATE', \r\n" +
                               $"  '{product}', \r\n" +
                               $"  '{tmp}' \r\n" +
                                ") \r\n";
                            ExecuteNonQuery(sql);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("SetSent error: " + ex.Message + "\r\n" + sql, ex);
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
                sql =
                    "select ini_value \r\n" +
                    "  from sys_ini \r\n" +
                    " where ini_file_name = 'SYLICCLN' \r\n" +
                    "   and ini_section = 'WebService' \r\n";
                using (var rdr = ExecuteReader(sql))
                {
                    if (rdr.Read())
                    {
                        string tmp = rdr["ini_value"] is DBNull ? "" : rdr["ini_value"].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(tmp))
                        {
                            LicenseUrl = tmp;
                            return;
                        }
                    }
                }
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
                        result.Add(new UpdateUnitDTO { UnitName = "User: " + name, UnitCount = 1 });
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
                        result.Add(new UpdateUnitDTO { UnitName = "Truck: " + name, UnitCount = 1 });
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
                    " order by 1";
                using (var rdr = ExecuteReader(sql))
                {
                    while (rdr.Read())
                    {
                        int truck = rdr["sct_truck"] is DBNull ? -1 : (int)rdr["sct_truck"];
                        result.Add(new UpdateUnitDTO { UnitName = "Truck: " + truck.ToString(), UnitCount = 1 });
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

        void GetArchiveFolder()
        {
            try
            {
                string wrkFolder = GetWorkingFolder();
                string dbName = GetDatabaseName();
                if (!string.IsNullOrWhiteSpace(wrkFolder) && !string.IsNullOrWhiteSpace(dbName))
                {
                    string folder = System.IO.Path.Combine(wrkFolder, dbName, "SYLICCLN");
                    if (!System.IO.Directory.Exists(folder))
                        System.IO.Directory.CreateDirectory(folder);
                    ArchiveFolder = folder;
                }
            }
            catch (Exception ex)
            {
                Tools.Logger.Entry(new ApplicationException($"Error in GetArchiveFolder: {ex.Message}", ex));
                ArchiveFolder = "";
            }
        }

        string GetWorkingFolder()
        {
            // HOSTED?
            string folder = GetHostedWorkingFolder();
            if (string.IsNullOrWhiteSpace(folder))
            {
                try
                {
                    // NON HOSTED
                    string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string appPath = Uri.UnescapeDataString(uri.Path);
                    appPath = System.IO.Path.GetDirectoryName(appPath);

                    int idx = appPath.IndexOf(@"\EXECTRAK\");
                    folder = idx > 0 ? appPath.Substring(0, idx + "\\EXECTRAK".Length) : appPath.TrimEnd(" \\".ToCharArray()) + "\\EXECTRAK";
                    folder = folder.TrimEnd(" \\".ToCharArray());
                    if (!System.IO.Directory.Exists(folder))
                        System.IO.Directory.CreateDirectory(folder);
                }
                catch (Exception ex)
                {
                    Tools.Logger.Entry(new ApplicationException($"Error in GetWorkingFolder: {ex.Message}", ex));
                    folder = "";
                }
            }

            return folder;
        }

        string GetHostedWorkingFolder()
        {
            string folder = "";
            string sql = "";
            try
            {
                string parm15 = "N";
                sql =
                    "select parm_field \r\n" +
                    "  from sys_parm \r\n" +
                    " where parm_nbr = 15 \r\n";
                using (var rdr = ExecuteReader(sql))
                {
                    if (rdr.Read())
                    {
                        parm15 = rdr["parm_field"] is DBNull ? "" : rdr["parm_field"].ToString().Trim().ToUpper();
                    }
                }

                if (parm15 != "Y")
                    return folder;

                string wrkDir = "";
                sql =
                    "select ini_value \r\n" +
                    "  from sys_ini \r\n" +
                    " where ini_file_name = 'HOSTED' \r\n" +
                    "   and ini_section = 'WorkingDirectory' \r\n";
                using (var rdr = ExecuteReader(sql))
                {
                    if (rdr.Read())
                    {
                        wrkDir = rdr["ini_value"] is DBNull ? "" : rdr["ini_value"].ToString().Trim().ToUpper();
                    }
                }

                if (System.IO.Directory.Exists(wrkDir))
                    folder = wrkDir;
            }
            catch (Exception ex)
            {
                Tools.Logger.Entry(new ApplicationException($"Error in GetHostedWorkingFolder: {ex.Message} \r\n{sql}", ex));
                folder = "";
            }

            return folder;
        }

        string GetDatabaseName()
        {
            string name = "";
            try
            {
                name = _con.Database.Trim().ToUpper();
                if (name.EndsWith("/FACTOR"))
                    name = name.Substring(0, name.Length - "/FACTOR".Length);
                if (name.Contains("/"))
                {
                    string[] parts = name.Split('/');
                    for(int i = parts.Length - 1; i >= 0; i--)
                    {
                        if (!string.IsNullOrWhiteSpace(parts[i]))
                        {
                            name = parts[i];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.Logger.Entry(new ApplicationException($"Error in GetDatabaseName: {ex.Message}", ex));
            }

            return name;
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
