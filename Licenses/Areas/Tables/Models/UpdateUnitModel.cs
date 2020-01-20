using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Models
{
    public class UpdateUnitModel
    {
        public UpdateUnitModel()
        {

        }

        public UpdateUnitModel(System.Data.IDataReader rdr)
        {
            updunit_id = (long)rdr["updunit_id"];
            updunit_upd_id = (long)rdr["updunit_upd_id"];
            updunit_unit_id = (long)rdr["updunit_unit_id"];
            updunit_count = (long)rdr["updunit_count"];
        }

        public UpdateUnitModel(FormCollection collection)
        {
            Update(collection);
        }

        #region Database Properties

        [DisplayName("Record Id")]
        public long updunit_id { get; set; }

        [DisplayName("Update Id")]
        public long updunit_upd_id { get; set; }

        [Required(ErrorMessage = "Update Unit Id is required")]
        [DisplayName("Update Unit Id")]
        public long updunit_unit_id { get; set; }

        [Required(ErrorMessage = "Update Unit Count is required")]
        [DisplayName("Update Unit Count")]
        public long updunit_count { get; set; }

        #endregion Database Properties

        #region UI Properties

        [DisplayName("Unit Name")]
        public string UnitName { get; set; }

        [DisplayName("Update Date")]
        public DateTime UpdateDate { get; set; }

        [DisplayName("Client Name")]
        public string ClientName { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        public List<SelectListItem> AvailableUnits { get; set; } = new List<SelectListItem>();

        public void Update(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "updunit_id": updunit_id = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                    case "updunit_upd_id": updunit_upd_id = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                    case "updunit_unit_id": updunit_unit_id = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                    case "updunit_count": updunit_count = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                }
            }
        }

        #endregion UI Properties
    }
}