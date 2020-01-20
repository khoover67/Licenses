using Licenses.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licenses.Library.Encryption
{
    public class Auth
    {
        const string key = "T0Rn4do!";

        public static string GetToken(string user)
        {
            string plain = key + DateTime.UtcNow.ToString("yyyyMMddHHmm") + user;
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

            TimeSpan ts = DateTime.UtcNow.Subtract(dt);
            if (ts.Minutes < 0 || ts.Minutes > 10)
                return user;

            user = plain.Substring(key.Length + 12);
            return user;
        }

        public static string ValidateDTO(IDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException("Empty dto passed to ValidateDTO");
            if (string.IsNullOrWhiteSpace(dto.AuthToken))
                throw new ArgumentNullException("dto with no credentials passed to ValidateDTO");
            string user = IsValidToken(dto.AuthToken);
            if (string.IsNullOrWhiteSpace(user))
                throw new ApplicationException("Invalid Credentials");

            return user;
        }
    }
}
