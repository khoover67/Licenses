using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Licenses.DataAccess
{
    public static class Factory
    {
        public static ILicensesAccess GetLicensesAccess()
        {
            string access = ConfigurationManager.AppSettings["DataAccess"] ?? "SQLite";

            switch (access)
            {
                //case "Testing":
                //    return new Testing.ClientAccess();
                //case "JsonFile":
                //    return JsonFile.ClientAccess.Instance;
                case "SQLite":
                    return new SQLite.LicensesAccess();
                default:
                    throw new ApplicationException("No Client Access found for '" + access + "'");
            }
        }

        public static ISettingAccess GetSettingAccess()
        {
            string access = ConfigurationManager.AppSettings["DataAccess"] ?? "Testing";

            switch (access)
            {
                case "SQLite":
                    return new SQLite.SettingAccess();
                default:
                    throw new ApplicationException("No Client Access found for '" + access + "'");
            }
        }
    }
}