using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Licenses.Models
{
    public class ClientListModel
    {
        public List<ClientModel> Clients { get; set; } = new List<ClientModel>();
    }
}