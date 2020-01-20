using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Licenses.Models
{
    public class SettingListModel
    {
        public string AuthToken { get; set; }

        public List<SettingModel> Settings { get; set; } = new List<SettingModel>();
    }
}