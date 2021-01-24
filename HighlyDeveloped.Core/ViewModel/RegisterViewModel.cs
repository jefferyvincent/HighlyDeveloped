using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlyDeveloped.Core.ViewModel
{
    /// <summary>
    ///  This is the view model for the registration page 
    /// </summary>
    public class RegisterViewModel
    {
        [DisplayName("First Name")]
        [Required(ErrorMessage ="Please enter your first name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }
        [DisplayName("User Name")]
        [Required(ErrorMessage = "Please enter your user name")]
        [MinLength(6)]
        public string Username { get; set; }
        [DisplayName("Email")]
        [Required(ErrorMessage = "Please enter your user email")]
        public string EmailAddress { get; set; }
        [UIHint("Password")]
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please enter your user password")]
        [MinLength(10, ErrorMessage = "Please enter your user password at least 10 characters")]
        public string Password { get; set; }
        [UIHint("Confirm Password")]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please re-enter your user password")]
        [EqualTo("Password", ErrorMessage ="Please ensure your passwords match")]
        public string ConfirmPassword { get; set; }
    }
}
