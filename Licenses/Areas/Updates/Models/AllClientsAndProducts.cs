using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Updates.Models
{
    public class AllClientsAndProducts
    {
        public List<ClientModel> Clients { get; set; } = new List<ClientModel>();

        public List<ProductModel> Products { get; set; } = new List<ProductModel>();
    }
}