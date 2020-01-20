using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Licenses.Models
{
    public class AllClientModel
    {
        public string Name { get; set; }

        public string Database { get; set; }

        [DisplayName("Report Count")]
        public int ReportCount { get; set; } 
    }
}