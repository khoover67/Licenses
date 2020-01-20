using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Tables.DataAccess
{
    public static class Factory
    {
        public static ITableAccess GetTableAccess()
        {
            string access = ConfigurationManager.AppSettings["DataAccess"] ?? "SQLite";

            switch (access)
            {
                //case "Testing":
                //    return new Testing.ClientAccess();
                case "SQLite":
                    return new SQLite.TableAccess();
                default:
                    throw new ApplicationException("No Client Access found for '" + access + "'");
            }
        }
    }
}