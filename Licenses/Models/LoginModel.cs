using Licenses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Licenses.Models
{
    public class LoginModel
    {
        public LoginModel()
        {
        }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}