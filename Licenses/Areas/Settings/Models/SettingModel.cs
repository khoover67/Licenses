using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Licenses.Areas.Settings.Models
{
    public class SettingModel
    {
        [Required(ErrorMessage = "Setting Name is required")]
        [DisplayName("Setting Name")]
        public string set_name { get; set; }

        [Required(ErrorMessage = "Setting Value is required")]
        [DisplayName("Setting Value")]
        public string set_value { get; set; }
    }
}