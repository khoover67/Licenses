using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenses.Areas.Tables.Models
{
    public class UpdateCountSearchModel
    {
        public UpdateCountSearchModel()
        {

        }

        public UpdateCountSearchModel(FormCollection collection)
        {
            foreach (string key in collection.AllKeys)
            {
                switch (key)
                {
                    case "StartDate": StartDate = (DateTime)collection.GetValue(key).ConvertTo(typeof(DateTime)); break;
                    case "EndDate": EndDate = (DateTime)collection.GetValue(key).ConvertTo(typeof(DateTime)); break;
                    case "ClientId": ClientId = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                    case "ProductId": ProductId = (long)collection.GetValue(key).ConvertTo(typeof(long)); break;
                }
            }
        }

        [Required(ErrorMessage = "Start Date is required")]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }

        [DisplayName("Client")]
        public long ClientId { get; set; }

        [DisplayName("Product")]
        public long ProductId { get; set; }

        public List<SelectListItem> AvailableClients { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> AvailableProducts { get; set; } = new List<SelectListItem>();

        public List<UpdateCountModel> Updates { get; set; } = new List<UpdateCountModel>();
    }
}