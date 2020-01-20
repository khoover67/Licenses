using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Updates.Models
{
    public class UpdateDetailsModel
    {
        public UpdateDetailsModel()
        {

        }

        public string Date { get; set; }

        public string Account { get; set; }

        public string Client { get; set; }

        public string Database { get; set; }

        public string Product { get; set; }

        public string Unit { get; set; }

        public long Count { get; set; }
    }
}