using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Updates.DataAccess
{
    public static class Factory
    {
        public static IUpdateAccess GetUpdateAccess()
        {
            string access = ConfigurationManager.AppSettings["DataAccess"] ?? "SQLite";

            switch (access)
            {
                //case "Testing":
                //    return new Testing.ClientAccess();
                case "SQLite":
                    return new SQLite.UpdateAccess();
                default:
                    throw new ApplicationException("No Client Access found for '" + access + "'");
            }
        }
    }
}