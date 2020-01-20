using Licenses.Areas.Tables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.DataAccess
{
    public interface ITableAccess : IDisposable
    {
        #region Account Methods

        int AddAccount(AccountModel account);

        int DeleteAccount(long account_id);

        AccountModel GetAccount(long account_id);

        long GetAccountId(string name);

        long GetAccountId(long account_number);

        List<AccountModel> GetAccounts();

        int UpdateAccount(long account_id, FormCollection collection);

        #endregion Account Methods

        #region Client Methods

        int AddClient(ClientModel client);

        int DeleteClient(long cln_id);

        ClientModel GetClient(long cln_id, bool fillAvailableLists = true);

        long GetClientId(string name, string database);

        List<ClientModel> GetClients();

        int UpdateClient(long cln_id, FormCollection collection);

        #endregion Client Methods

        #region Product Methods

        int AddProduct(ProductModel product);

        int DeleteProduct(long prod_id);

        ProductModel GetProduct(long prod_id);

        long GetProductId(string name);

        List<ProductModel> GetProducts();

        int UpdateProduct(long prod_id, FormCollection collection);

        #endregion Product Methods
        
        #region Unit Methods

        int AddUnit(UnitModel unit);

        int DeleteUnit(long unit_id);

        UnitModel GetUnit(long unit_id);

        long GetUnitId(string name);

        List<UnitModel> GetUnits();

        int UpdateUnit(long unit_id, FormCollection collection);

        #endregion Unit Methods

        #region Update Count Methods

        int AddUpdateCount(UpdateCountModel updateCount);

        int DeleteUpdateCount(long upd_id);

        UpdateCountModel GetUpdateCount(long upd_id, bool fillAvailableLists = true);

        List<UpdateCountModel> GetUpdateCounts(DateTime dtStart, DateTime dtEnd, long idClient = 0, long idProduct = 0);

        int UpdateUpdateCount(long upd_id, FormCollection collection);

        #endregion Update Count Methods

        #region Update Unit Methods

        int AddUpdateUnit(UpdateUnitModel updateUnit);

        int DeleteUpdateUnit(long updunit_id);

        UpdateCountModel GetUpdateCountWithUnits(long upd_id);

        UpdateUnitModel GetUpdateUnit(long updunit_id);

        int UpdateUpdateUnit(long updunit_id, FormCollection collection);

        #endregion Update Unit Methods

        #region Helper Methods

        List<SelectListItem> GetAvailableAccounts();

        List<SelectListItem> GetAvailableClients(bool allowEmpty = false);

        List<SelectListItem> GetAvailableProducts(bool allowEmpty = false);

        List<SelectListItem> GetAvailableUnits();

        #endregion Helper Methods
    }
}
