using Licenses.Library.DTO;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licenses.Library.Client
{
    public static class Tools
    {
        public static UpdateCountDTO CreateUpdateCountDTO(
            string user,
            string clientName,
            string databasePathe,
            string productName,
            DateTime dt,
            List<UpdateUnitDTO> units,
            bool dateIsUniversal = false)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                clientName = "<Empty Client Passed>";
            if (string.IsNullOrWhiteSpace(productName))
                productName = "<Empty Product Passed>";

            if (!dateIsUniversal)
                dt = dt.ToUniversalTime();

            UpdateCountDTO update = new UpdateCountDTO
            {
                AuthToken = Encryption.Auth.GetToken(user),
                ClientName = string.IsNullOrWhiteSpace(clientName) ? "<Empty Client Passed>" : clientName,
                DatabasePath = databasePathe,
                ProductName = productName,
                Units = units ?? new List<UpdateUnitDTO> { new UpdateUnitDTO { UnitCount = 0, UnitName = "<No Units Passed>" } },
                Date = dt
            };

            return update;
        }

        public static UpdateCountDTO CreateUpdateCountDTO(
            string connectionString, 
            string clientName,
            string productName,
            List<UpdateUnitDTO> units,
            DateTime dt,
            bool dateIsUniversal = false)
        {
            using (OdbcConnection con = new OdbcConnection(connectionString))
            {
                con.Open();
                return CreateUpdateCountDTO(con, clientName, productName, units, dt, dateIsUniversal);
            }
        }

        /// <summary>
        /// For testing only. Don't use this in production.
        /// </summary>
        /// <param name="con"></param>
        /// <param name="clientName"></param>
        /// <param name="productName"></param>
        /// <param name="units"></param>
        /// <param name="dt"></param>
        /// <param name="dateIsUniversal"></param>
        /// <returns></returns>
        public static UpdateCountDTO CreateUpdateCountDTO(
            OdbcConnection con,
            string clientName,
            string productName, 
            List<UpdateUnitDTO> units,
            DateTime dt,
            bool dateIsUniversal)
        {
            string user = ParseUser(con.ConnectionString);
            if (string.IsNullOrWhiteSpace(productName))
                productName = "<Empty Product Passed>";

            if (!dateIsUniversal)
                dt = dt.ToUniversalTime();

            UpdateCountDTO update = new UpdateCountDTO
            {
                AuthToken = Encryption.Auth.GetToken(user),
                ClientName = string.IsNullOrWhiteSpace(clientName) ? "<Empty Client Passed>" : clientName,
                DatabasePath = con.Database,
                ProductName = productName,
                Units = units ?? new List<UpdateUnitDTO> { new UpdateUnitDTO { UnitCount = 0, UnitName = "<Empty Unit Passed>" } },
                Date = dt
            };

            using (OdbcCommand cmd = new OdbcCommand("select con_name from co_company_name", con))
            {
                using (OdbcDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        update.ClientName = rdr["con_name"] is System.DBNull ? "<No Name Found>" : rdr["con_name"].ToString().Trim();
                    }
                }
            }

            return update;
        }

        public static string ParseUser(string connectionString)
        {
            string user = "<User Not Found>";
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                string[] parts = connectionString.Split(';');
                foreach(string part in parts)
                {
                    int eq = part.IndexOf('=');
                    if (eq > 0)
                    {
                        string name = part.Substring(0, eq).Trim().ToUpper();
                        string value = part.Substring(eq + 1).Trim();
                        if (!string.IsNullOrEmpty(value))
                        {
                            switch (name)
                            {
                                case "UID":
                                case "USER ID":
                                    return value;
                            }
                        }
                    }
                }
            }

            return user;
        }
    }
}
