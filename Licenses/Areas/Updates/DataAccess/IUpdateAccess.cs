using Licenses.Areas.Updates.Models;
using System;
using System.Collections.Generic;

namespace Licenses.Areas.Updates.DataAccess
{
    public interface IUpdateAccess : IDisposable
    {
        #region Client

        //List<ClientModel> GetAllClients();

        ClientModel GetClient(long cln_id);

        AllClientsAndProducts GetAllClientsAndProducts();

        List<ClientModel> GetLatestUpdatesByClient();

        List<UpdateCountModel> GetUpdatesForPaging(long idClient, long idProduct, int page, out int totalPages, int pageSize = 25, bool asc = true);

        List<UpdateDetailsModel> GetUpdateDetails(DateTime dtStart, DateTime dtEnd);

        List<List<string>> GetUpdateDetailsForJson(DateTime dtStart, DateTime dtEnd);

        #endregion Client
    }
}
