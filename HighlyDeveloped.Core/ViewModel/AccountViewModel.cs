using DataAnnotationsExtensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HighlyDeveloped.Core.ViewModel
{
    /// <summary>
    /// This is the view model for the account page
    /// </summary>
    public class AccountViewModel
    {
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "Please your name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please your email address")]
        [Email(ErrorMessage="Please enter a valid email address")]
        public string Email { get; set; }
        public string Username { get; set; }
        [UIHint("Password")]
        [DisplayName("Password")]
        [MinLength(10, ErrorMessage = "Please enter your user password at least 10 characters")]
        public string Password { get; set; }
        [UIHint("Confirm Password")]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please re-enter your user password")]
        [EqualTo("Password", ErrorMessage = "Please ensure your passwords match")]
        public string ConfirmPassword { get; set; }
    }
}
