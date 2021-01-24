using HighlyDeveloped.Core.Interfaces;
using HighlyDeveloped.Core.ViewModel;
using System;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Core.Logging;
using System.Linq;

namespace HighlyDeveloped.Core.Controllers
{
    public class LoginController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Login/";
        private IEmailService _emailService;
        public LoginController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        #region Login
        public ActionResult RenderLogin()
        {
            var vm = new LoginViewModel();
            vm.RedirectUrl = HttpContext.Request.Url.AbsolutePath;
            return PartialView(PARTIAL_VIEW_FOLDER + "Login.cshtml", vm);
        }
        /// <summary>
        /// Not using Built in Umbraco, but custom ASP.NET one
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleLogin(LoginViewModel vm)
        {
            // Check if model is ok
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            // check if member exists with username
            var member = Services.MemberService.GetByUsername(vm.UserName);
            if (member == null)
            {
                ModelState.AddModelError("Login", "Cannot find user in the system");
                return CurrentUmbracoPage();
            }
            // check if member is locked out
            if (member.IsLockedOut)
            {
                ModelState.AddModelError("Login", "The account is locked please use the forgetten password page");
                return CurrentUmbracoPage();
            }
            // check if they validated there email address
            var emailVerified = member.GetValue<bool>("emailVerified");
            if (!emailVerified)
            {
                ModelState.AddModelError("Login", "Please verifiy your email before logging in");
                return CurrentUmbracoPage();
            }
            // Check if credentials are ok
            // log them in
            if (!Members.Login(vm.UserName, vm.Password))
            {
                ModelState.AddModelError("Login", "The username/password your provided is not correct");
                return CurrentUmbracoPage();
            }

            if (!string.IsNullOrEmpty(vm.RedirectUrl))
            {
                return Redirect(vm.RedirectUrl);
            }
            return RedirectToCurrentUmbracoPage();
        }
        #endregion
        #region Forgotten Password
        public ActionResult RenderForgottenPassword()
        {
            var vm = new ForgottenPasswordViewModel();
            return PartialView(PARTIAL_VIEW_FOLDER + "ForgottenPassword.cshtml", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleForgottenPassword(ForgottenPasswordViewModel vm)
        {
            // is the model ok?
            if(!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            // Do we have a memember with this email address?
            // if not, Error
            var member = Services.MemberService.GetByEmail(vm.EmailAddress);
            if(member == null)
            {
                ModelState.AddModelError("Error", "Sorry can't find the email in the system");
                return CurrentUmbracoPage();
            }
            // Create the reset token
            var resetToken = Guid.NewGuid().ToString();

            //Set the reset expiry date (now + 12 hours)
            var expiryDate = DateTime.Now.AddHours(12);
            // save memeber
            member.SetValue("resetExpiryDate", expiryDate);
            member.SetValue("resetLinkToken", resetToken);
            Services.MemberService.Save(member);

            // Fire the email - reset password
            _emailService.SendResetPasswordNotification(vm.EmailAddress, resetToken);
            Logger.Info<LoginController>($"Sent a password reset to {vm.EmailAddress}");

            // Thanks
            TempData["status"] = "OK";
            return RedirectToCurrentUmbracoPage();
        }
        #endregion
        #region Reset Password
        public ActionResult RenderResetPassword()
        {
            var vm = new ResetPasswordViewModel();
            return PartialView(PARTIAL_VIEW_FOLDER + "ResetPassword.cshtml", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleResetPassword(ResetPasswordViewModel vm)
        {
            // get reset token
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            
            // Ensure that we have a token
            var token = Request.QueryString["token"];
            if(string.IsNullOrEmpty(token))
            {
                Logger.Warn<LoginController>("Reset Password - no token found");
                ModelState.AddModelError("Error", "Invalid Token");
                return CurrentUmbracoPage();
            }
            
            // get the member for the token
            var member = Services.MemberService.GetMembersByPropertyValue("resetLinkToken", token).SingleOrDefault();
            if(member == null)
            {
                ModelState.AddModelError("Error", "That link is no longer valid");
                return CurrentUmbracoPage();
            }
            
            // check for the time window hasn't expired
            var memberTokenExpiryDate = member.GetValue<DateTime>("resetExpiryDate");
            var currentTime = DateTime.Now;
            if(currentTime.CompareTo(memberTokenExpiryDate) >= 0)
            {
                ModelState.AddModelError("Error", "That link is no longer valid and has expired.");
                return CurrentUmbracoPage();
            }
            
            // if ok, update the password for the member
            Services.MemberService.SavePassword(member, vm.Password);
            member.SetValue("resetLinkToken", string.Empty);
            member.SetValue("resetExpiryDate", null);
            member.IsLockedOut = false;
            Services.MemberService.Save(member);
            
            // send out password changed email
            _emailService.SendPasswordChangedNotification(member.Email);

            // Give thanks
            TempData["status"] = "OK";
            Logger.Info<LoginController>($"User {member.Username} has changed their password.");
            
            return RedirectToCurrentUmbracoPage();
        }
        #endregion
    }
}
