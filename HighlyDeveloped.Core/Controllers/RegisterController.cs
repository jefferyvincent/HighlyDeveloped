using HighlyDeveloped.Core.Interfaces;
using HighlyDeveloped.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace HighlyDeveloped.Core.Controllers
{
    public class RegisterController: SurfaceController
    {
        /// <summary>
        /// This will render the registration form
        /// </summary>
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/";
        private IEmailService _emailService;

        public RegisterController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        #region Register Form
        public ActionResult RenderRegister()
        {
            var vm = new RegisterViewModel();
            return PartialView(PARTIAL_VIEW_FOLDER + "Register.cshtml", vm);
        }
        /// <summary>
        /// This will handle the registration form post
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleRegister(RegisterViewModel vm)
        {
            // If form not vaild - return
            if(!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            // Check if there is already a member with that email
            var existingMember = Services.MemberService.GetByEmail(vm.EmailAddress);

            if(existingMember !=null)
            {
                ModelState.AddModelError("Account Error", "There is already a user with that email address");
                return CurrentUmbracoPage();
            }
            // Check if their username is already in use
            existingMember = Services.MemberService.GetByUsername(vm.Username);
            if (existingMember != null)
            {
                ModelState.AddModelError("Account Error", "There is already a user with that user name. Please choose a differnt one.");
                return CurrentUmbracoPage();
            }
            // Create "member" in Umbraco with the details
            var newMember = Services.MemberService.CreateMember(vm.Username, vm.EmailAddress, $"{vm.FirstName} {vm.LastName}", "Member");
            newMember.PasswordQuestion = "";
            newMember.RawPasswordAnswerValue = "";

            // need to save member before setting password
            Services.MemberService.Save(newMember);
            Services.MemberService.SavePassword(newMember, vm.Password);

            // Assign a role - i.e Normal User
            Services.MemberService.AssignRole(newMember.Id, "Normal User");

            // Create email verification token
            // token creation
            var token = Guid.NewGuid().ToString();
            newMember.SetValue("emailVerifyToken", token);
            Services.MemberService.Save(newMember);

            // Send email verification
            _emailService.SendVerifyEmailAddressNotification(newMember.Email, token);

            // Thank the user
            // return confirmation message to user
            TempData["status"] = "Ok";

            return RedirectToCurrentUmbracoPage();
        }
        #endregion
        #region Verification
        public ActionResult RenderEmailVerification(string token)
        {
            // Get token (query string) - Controller automatically pulls the query string
            // Look for a member matching this token
            var member = Services.MemberService.GetMembersByPropertyValue("emailVerifyToken", token).SingleOrDefault();
            
            if(member != null)
            {
                // If we find one, set them to verified
                var alreadyVerified = member.GetValue<bool>("emailVerified");
                if(alreadyVerified)
                {
                    ModelState.AddModelError("Verified", "Your already verified your email address!");
                    return CurrentUmbracoPage();
                }
                member.SetValue("emailVerified", true);
                member.SetValue("emailVerifiedDate", DateTime.Now);
                Services.MemberService.Save(member);
                //Thank the user
                TempData["status"] = "Ok";
                return PartialView(PARTIAL_VIEW_FOLDER + "EmailVerification.cshtml");
            }


            // Otherwise...some problem
            ModelState.AddModelError("Error", "Appologies, Some other error occured!");
            return PartialView(PARTIAL_VIEW_FOLDER + "EmailVerification.cshtml");
        }
        #endregion
    }
}
