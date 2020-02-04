using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYLICCLN
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args == null)
                    throw new ApplicationException("Null args passed to Main - No command line");
                if (args.Length < 1)
                    throw new ApplicationException("Empty args passed to Main - No command line");

                SetConnection(args[0]);
                using (License lic = new License(Connection))
                {
                    lic.FlagExecuTrakUser();
                    lic.DoUpdates();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public static string Connection { get; private set; }

        public static void HandleException(Exception ex)
        {
            try
            {
                if (ex == null)
                    return;
                Tools.Logger.Entry(ex);
                Console.WriteLine(ex.Message);
            }
            catch { }
        }

        public static void SetConnection(string connection)
        {
            try
            {
                using (System.Data.Odbc.OdbcConnection con = new System.Data.Odbc.OdbcConnection(connection))
                {
                    con.Open();
                }

                Connection = connection;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Connection Test Failed for connection string: \r\n{connection}\r\n", ex);
            }
        }
    }
}
