using HighlyDeveloped.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Mvc;

namespace HighlyDeveloped.Core.Controllers
{
    public class AccountController: SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/MyAccount/";
        public ActionResult RenderMyAccount()
        {
            var vm = new AccountViewModel();
            // are we logged in?
            if (!Umbraco.MemberIsLoggedOn())
            {
                ModelState.AddModelError("Error", "You are not currently logged in.");
                return CurrentUmbracoPage();
            }
            // get members details
            var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName);
            if (member == null)
            {
                ModelState.AddModelError("Error", "We cannot find you in the system");
                return CurrentUmbracoPage();
            }
            // populate the view model
            vm.Name = member.Name;
            vm.Email = member.Email;
            vm.Username = member.Username;

            return PartialView(PARTIAL_VIEW_FOLDER + "MyAccount.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleUpdateDetails(AccountViewModel vm)
        {
            // Is this model valid?
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "There has been a problem with the model");
                return CurrentUmbracoPage();
            }
            // Is there a member?
            if(!Umbraco.MemberIsLoggedOn() || Membership.GetUser() == null)
            {
                ModelState.AddModelError("Error", "You are not logged on.");
                return CurrentUmbracoPage();
            }
            var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName);
            if (member == null)
            {
                ModelState.AddModelError("Error", "There has been a problem with member");
                return CurrentUmbracoPage();
            }
            // update the member's details
            member.Name = vm.Name;
            member.Email = vm.Email;

            Services.MemberService.Save(member);
            // thanks
            TempData["status"] = "Updated Details";
            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult HandlePasswordChange(AccountViewModel vm)
        {
            // Is model valid?
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "There has been a problem with the model");
                return CurrentUmbracoPage();
            }
            // Is there a member?
            if (!Umbraco.MemberIsLoggedOn() || Membership.GetUser() == null)
            {
                ModelState.AddModelError("Error", "You are not logged on.");
                return CurrentUmbracoPage();
            }
            var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName);
            if (member == null)
            {
                ModelState.AddModelError("Error", "There has been a problem with member");
                return CurrentUmbracoPage();
            }
            // Thanks - Updated password.
            try
            {
                Services.MemberService.SavePassword(member, vm.Password);
            }
            catch (MembershipPasswordException exc)
            {
                ModelState.AddModelError("Error", "There has been a problem with your password" + exc.Message);
                return CurrentUmbracoPage();
            }

            TempData["status"] = "Updated Password";
            return RedirectToCurrentUmbracoPage();
        }
    }
}
