using Licenses.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Licenses.DataAccess
{
    public interface ILicensesAccess : IDisposable
    {
        long AddUpdateCount(DateTime dateTime, string clientName, string dbPath, string productName, SortedList<string, int> units, DateTime date);

        DateTime LastUpdate(string clientName, string dbPath, string productName);
    }
}
