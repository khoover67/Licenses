using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licenses.Library.DTO
{
    public class ResultDTO
    {
        public const string SUCCESS = "Success";
        public const string INTERVAL = "Interval";

        public string Message { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorStack { get; set; }

        public bool Succeeded()
        {
            return Message == SUCCESS || Message == INTERVAL;
        }
    }
}
