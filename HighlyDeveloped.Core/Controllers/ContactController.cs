using HighlyDeveloped.Core.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Core.Logging;
using HighlyDeveloped.Core.Interfaces;

namespace HighlyDeveloped.Core.Controllers
{
    public class ContactController: SurfaceController
    {
        private IEmailService _emailService;

        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public ActionResult RenderContactForm()
        {
            var vm = new ContactFormViewModel();
            return PartialView("~/Views/Partials/ContactFormTemplate.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleContactForm(ContactFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "Please check the form");
                return CurrentUmbracoPage();
            }


            try
            {
                // Create a new contact form in umbraco
                // Get a handle to "Contact Forms"
                var contactForms = Umbraco.ContentAtRoot().DescendantsOrSelfOfType("ContactForms").FirstOrDefault();

                if (contactForms != null)
                {
                    var newContact = Services.ContentService.Create("Contact", contactForms.Id, "contactForm");
                    newContact.SetValue("contactName", vm.Name);
                    newContact.SetValue("contactEmail", vm.EmailAddress);
                    newContact.SetValue("contactSubject", vm.Subject);
                    newContact.SetValue("contactComments", vm.Comment);
                    Services.ContentService.SaveAndPublish(newContact);
                }

                // send email to site admin
                _emailService.SendContactNotificationToAdmin(vm);

                // return confirmation message to user
                TempData["status"] = "Ok";

                return RedirectToCurrentUmbracoPage(); 
            }
            catch (Exception exc)
            {
                Logger.Error<ContactController>("There was an error in the contact form submission", exc.Message);
                ModelState.AddModelError("Error", "Sorry there was a problem noting your details, would you please try again later?");
            }

            return CurrentUmbracoPage();
        }
    }
}
