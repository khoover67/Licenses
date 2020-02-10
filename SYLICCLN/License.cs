using Licenses.Library.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SYLICCLN
{
    class License : IDisposable
    {
        //public const string EXECUTRAK = "EXECUTRAK";
        //public const string DEALERTRAK = "DEALERTRAK";
        //public const string DELIVERYTRAK = "DELIVERYTRAK";
        //public const string FUELTRAK = "FUELTRAK";
        //public const string STORETRAK = "STORETRAK";

        XmlSerializer _xmlSerializer;

        public enum Products
        {
            DealerTrak,
            DeliveryTrak,
            ExecuTrak,
            FuelTrak,
            StoreTrak
        }

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
            foreach(Products product in (Products[])Enum.GetValues(typeof(Products)))
            {
                if (!ProductWasSentToday(product))
                {
                    List<UpdateUnitDTO> units = GetProductUnits(product);
                    string name = Enum.GetName(typeof(Products), product);
                    UpdateCountDTO dto = Licenses.Library.Client.Tools.CreateUpdateCountDTO(UserName, ClientName, _con.Database, name, dtSent, units);
                    ResultDTO result = SendUpdate(dto, product, dtSent);
                }
            }

            SendSavedCounts();
        }

        bool ProductWasSentToday(Products product)
        {
            switch (product)
            {
                case Products.DealerTrak:
                    return DealerTrakSent;
                case Products.DeliveryTrak:
                    return DeliveryTrakSent;
                case Products.ExecuTrak:
                    return ExecuTrakSent;
                case Products.FuelTrak:
                    return FuelTrakSent;
                case Products.StoreTrak:
                    return StoreTrakSent;
            }

            return true; // Unknown product, don't send it!
        }

        List<UpdateUnitDTO> GetProductUnits(Products product)
        {
            List<UpdateUnitDTO> units;
            switch (product)
            {
                case Products.DealerTrak:
                    units = GetDealerTrakUnits();
                    break;
                case Products.DeliveryTrak:
                    units = GetDeliveryTrakUnits();
                    break;
                case Products.ExecuTrak:
                    units = GetExecuTrakUnits();
                    break;
                case Products.FuelTrak:
                    units = GetFuelTrakUnits();
                    break;
                case Products.StoreTrak:
                    units = GetStoreTrakUnits();
                    break;
                default:
                    units = new List<UpdateUnitDTO>();
                    break;
            }

            return units;
        }

        ResultDTO SendUpdate(UpdateCountDTO dto, Products product, DateTime dtSent, bool archiveOnFail = true)
        {
            ResultDTO result;
            try
            {
                result = Licenses.Library.Server.Tools.PostToServer(dto, LicenseUrl, false);
                SetSent(product, dtSent);
            }
            catch (Exception ex)
            {
                Tools.Logger.Entry(new ApplicationException($"Failed to Send count info to '{LicenseUrl}': \r\n{ex.Message}", ex));
                if (archiveOnFail)
                    SaveCountToArchiveFile(dto, product);
                result = new ResultDTO { ErrorMessage = ex.Message, ErrorStack = ex.ToString() };
            }

            return result;
        }

        public void SaveCountToArchiveFile(UpdateCountDTO dto, Products product)
        {
            if (dto == null)
                throw new ArgumentNullException("Empty dto passed to SaveRquestToArchiveFile");

            string file = "";
            try
            {
                string name = Enum.GetName(typeof(Products), product);

                if (string.IsNullOrWhiteSpace(ArchiveFolder)
                    || !System.IO.Directory.Exists(ArchiveFolder))
                    return;

                file = System.IO.Path.Combine(ArchiveFolder,
                    name + "_" + DateTime.Today.ToString("yyyy-MM-dd") + ".xml");

                if (System.IO.File.Exists(file))
                    return;

                if (_xmlSerializer == null)
                    _xmlSerializer = new XmlSerializer(typeof(UpdateCountDTO));

                using (System.IO.TextWriter wtr = new System.IO.StreamWriter(file))
                {
                    _xmlSerializer.Serialize(wtr, dto);
                }
            }
            catch (Exception ex)
            {
                Tools.Logger.Entry(new ApplicationException($"Failed to archive count info to file {file}: \r\n{ex.Message}", ex));
            }
        }

        public int SendSavedCounts()
        {
            int cnt = 0;

            if (string.IsNullOrWhiteSpace(ArchiveFolder)
                || !System.IO.Directory.Exists(ArchiveFolder))
                return cnt;

            string[] files = System.IO.Directory.GetFiles(ArchiveFolder, "*.xml");
            if (files.Length < 1)
                return 0;

            if (_xmlSerializer == null)
                _xmlSerializer = new XmlSerializer(typeof(UpdateCountDTO));

            foreach (string file in files)
            {
                int uscore = file.IndexOf('_');
                if (uscore < 1)
                    continue;
                string[] parts = file.Split('_');
                if (parts.Length != 2)
                    continue;
                DateTime dt;
                if (!DateTime.TryParse(parts[1], out dt))
                    continue;
                TimeSpan ts = DateTime.Today.Subtract(dt);
                if (ts.Days > 30)
                {
                    try { System.IO.File.Delete(file); } catch { };
                    continue;
                }

                Products product;
                if (!Enum.TryParse(parts[0], out product))
                    continue;

                try
                {
                    UpdateCountDTO dto;
                    using (System.IO.Stream rdr = new System.IO.FileStream(file, System.IO.FileMode.Open))
                    {
                        dto = (UpdateCountDTO)_xmlSerializer.Deserialize(rdr);
                    }

                    dto.AuthToken = Licenses.Library.Encryption.Auth.GetToken(UserName);
                    SendUpdate(dto, product, dto.Date, false);
                    try { System.IO.File.Delete(file); } catch { };
                }
                catch (Exception ex)
                {
                    Tools.Logger.Entry(new ApplicationException($"Failed to send archived count file {file}: \r\n{ex.Message}", ex));
                }
            }

            return cnt;
        }

        void GetSent()
        {
            string sql = "";
            try
            {
                string[] products = Enum.GetNames(typeof(Products));
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
                        string date = rdr["ini_value"] is DBNull ? "" : rdr["ini_value"].ToString().Trim();
                        if (products.Contains(name))
                        {
                            Products product = (Products)Enum.Parse(typeof(Products), name);
                            bool sent = date == today;
                            if (sent)
                            {
                                switch (product)
                                {
                                    case Products.DealerTrak: DealerTrakSent = true; break;
                                    case Products.DeliveryTrak: DeliveryTrakSent = true; break;
                                    case Products.ExecuTrak: ExecuTrakSent = true; break;
                                    case Products.FuelTrak: FuelTrakSent = true; break;
                                    case Products.StoreTrak: StoreTrakSent = true; break;
                                }
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

        void SetSent(Products product, DateTime dtSent)
        {
            string sql = "";
            try
            {
                int cnt = 0;
                DateTime dtLatest = DateTime.MinValue;
                string name = Enum.GetName(typeof(Products), product);
                name = name.Replace("'", "''");

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
                        if (!int.TryParse(tmp, out cnt))
                            cnt = 0;

                        if (cnt > 0)
                        {
                            tmp = rdr["latest"] is DBNull ? "1900-01-01" : rdr["latest"].ToString().Trim();
                            if (!DateTime.TryParseExact(tmp, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtLatest))
                                dtLatest = DateTime.MinValue;
                        }
                    }
                }

                if (dtSent > dtLatest)
                {
                    string tmp = dtSent.ToString("yyyy-MM-dd");
                    if (cnt > 0)
                    {
                        sql =
                            "update sys_ini \r\n" +
                            $"   set ini_value = '{tmp}', \r\n" +
                            $"       ini_user_id = '{UserName.Replace("'", "''")}' \r\n" +
                            " where ini_file_name = 'SYLICCLN' \r\n" +
                            "   and ini_section = 'UPDATE' \r\n" +
                            $"   and ini_field_name = '{product}'";

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
                            $"  '{UserName ?? ""}', \r\n" +
                            "  'UPDATE', \r\n" +
                            $"  '{product}', \r\n" +
                            $"  '{tmp}' \r\n" +
                            ") \r\n";
                    }

                    ExecuteNonQuery(sql);
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
                   $"   and ini_user_id = 'EXECTRAK' \r\n" +
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
                    string folder = System.IO.Path.Combine(wrkFolder, "FACTOR", dbName, "SYLICCLN");
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
