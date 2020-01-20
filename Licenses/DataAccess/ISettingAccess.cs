using Licenses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Licenses.DataAccess
{
    public interface ISettingAccess : IDisposable
    {
        List<SettingModel> GetSettings();

        SettingModel GetSetting(string name);

        int AddSetting(SettingModel setting);

        int DeleteSetting(string name);

        int UpdateSetting(string name, FormCollection collection);
    }
}
