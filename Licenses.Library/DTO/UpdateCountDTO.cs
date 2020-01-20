using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licenses.Library.DTO
{
    public class UpdateCountDTO : IDTO
    {
        public int AccountId { get; set; }

        public string AuthToken { get; set; }

        public string ClientName { get; set; }

        public string DatabasePath { get; set; } 

        public string ProductName { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public List<UpdateUnitDTO> Units { get; set; } 
    }
}
