using Licenses.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Licenses.DataAccess
{
    public static class Auth
    {
        const string key = "6r3@tL!n3";

        public static string GetToken(string user)
        {
            string plain = key + DateTime.Now.ToString("yyyyMMddHHmm") + user;
            string token = Encrypt.EncryptString(plain);
            return token;
        }

        public static string IsValidToken(string token)
        {
            string user = null;

            if (string.IsNullOrWhiteSpace(token) || token.Length <= (key.Length + 12))
                return user;

            string plain = Encrypt.DecryptString(token);
            if (!plain.StartsWith(key))
                return user;

            //DateTime dt;
            string temp = plain.Substring(key.Length, 12);
            string year = temp.Substring(0, 4);
            string month = temp.Substring(4, 2);
            string day = temp.Substring(6, 2);
            string hour = temp.Substring(8, 2);
            string minute = temp.Substring(10, 2);
            string time = $"{month}/{day}/{year} {hour}:{minute}:00";
            if (!DateTime.TryParse(time, out DateTime dt))
                return user;

            int timeout = TokenTimeOut();
            TimeSpan ts = DateTime.Now.Subtract(dt);
            if (ts.Minutes < 0 || ts.Minutes > timeout)
                return user;

            user = plain.Substring(key.Length + 12);
            return user;
        }

        public static int TokenTimeOut()
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings["TokenTimeoutMinutes"];
                int timeout = int.Parse(value);
                if (timeout > 0)
                    return timeout;
                return 20;
            }
            catch (Exception ex)
            {
                Logger.Entry(ex);
                return 20;
            }
        }
    }
}