using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlyDeveloped.Core.ViewModel
{
    /// <summary>
    /// Th view model for the login from
    /// </summary>
   public class LoginViewModel
    {
        [Display(Name ="Username")]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
        public string RedirectUrl { get; set; }
    }
}
