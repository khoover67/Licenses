using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Updates.Models
{
    public class SearchClientProduct
    {
        [DisplayName("Client Name")]
        public long ClientId { get; set; }

        public List<SelectListItem> AvailableClients { get; set; } = new List<SelectListItem>();

        [DisplayName("Product Name")]
        public long ProductId { get; set; }

        public List<SelectListItem> AvailableProducts { get; set; } = new List<SelectListItem>();
    }
}