using Umbraco.Core.Logging;
using HighlyDeveloped.Core.Interfaces;
using HighlyDeveloped.Core.ViewModel;
using System;
using System.Linq;
using System.Net.Mail;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using System.Web;

namespace HighlyDeveloped.Core.Services
{
    /// <summary>
    ///  The home of all outbound emails to our site
    /// </summary>
    public class EmailService : IEmailService
    {
        private UmbracoHelper _umbraco;
        private IContentService _contentService;
        private ILogger _logger;

        public EmailService (UmbracoHelper umbraco, IContentService contentService, ILogger iLogger)
        {
            _umbraco = umbraco;
            _contentService = contentService;
            _logger = iLogger;
        }
        
        public void SendContactNotificationToAdmin(ContactFormViewModel vm)
        {
            //Get email template from Umbraco for "this" notification is
            var emailTemplate = GetEmailTemplate("New Contact Form Notification");

            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }

            //Get the template data
            var subject = emailTemplate.Value<string>("emailTemplateSubjectLine");
            var htmlContent = emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");

            //Mail merge the necessary fields
            //#name##
            MailMerge("name", vm.Name, ref htmlContent, ref textContent);
            //#email##
            MailMerge("email", vm.EmailAddress, ref htmlContent, ref textContent);
            //#comment##
            MailMerge("comment", vm.Comment, ref htmlContent, ref textContent);

            //Send email out to whoever
            //Read email FROM and TO addresses
            //Get site settings
            var siteSettings = _umbraco.ContentAtRoot().DescendantsOrSelfOfType("siteSettings").FirstOrDefault();
            if (siteSettings == null)
            {
                throw new Exception("There are no site settings");
            }

    
            var toAddresses = siteSettings.Value<string>("emailSettingsAdminAccounts");

            
            if (string.IsNullOrEmpty(toAddresses))
            {
                throw new Exception("There needs to be a to address in site settings");
            }

            SendMail(toAddresses, subject, htmlContent, textContent);

        }
        /// <summary>
        ///  A generic send mail that logs the email n umbraco and send via smtp
        /// </summary>
        /// <param name="toAddresses"></param>
        /// <param name="subject"></param>
        /// <param name="htmlContent"></param>
        /// <param name="textContent"></param>
        private void SendMail(string toAddresses, string subject, string htmlContent, string textContent)
        {
            //Get site settings
            var siteSettings = _umbraco.ContentAtRoot().DescendantsOrSelfOfType("siteSettings").FirstOrDefault();
            if (siteSettings == null)
            {
                throw new Exception("There are no site settings");
            }

            var fromAddress = siteSettings.Value<string>("emailSettingsFromAddress");
            if (string.IsNullOrEmpty(fromAddress))
            {
                throw new Exception("There needs to be a from address in site settings");
            }

            //Debug Mode
            var debugMode = siteSettings.Value<bool>("testMode");
            var testEmailAccounts = siteSettings.Value<string>("emailTestAccounts");

            var recipients = toAddresses;

            if (debugMode)
            {
                //Change the To - testEmailAccounts
                recipients = testEmailAccounts;
                //Alter subject line - to show in test mode
                subject += "(TEST MODE) - " + toAddresses;
            }

            //Log the email in umbraco
            //Emails
            //Email
            var emails = _umbraco.ContentAtRoot().DescendantsOrSelfOfType("emails").FirstOrDefault();
            var newEmail = _contentService.Create(toAddresses, emails.Id, "Email");
            newEmail.SetValue("emailSubject", subject);
            newEmail.SetValue("emailToAddress", recipients);
            newEmail.SetValue("emailHtmlContent", htmlContent);
            newEmail.SetValue("emailTextContent", textContent);
            newEmail.SetValue("emailSent", false);
            _contentService.SaveAndPublish(newEmail);

            // send the eamil via smptp or whatever
            var mimeType = new System.Net.Mime.ContentType("text/html");
            var alternateView = AlternateView.CreateAlternateViewFromString(htmlContent, mimeType);

            var smtpMessage = new MailMessage();
            smtpMessage.AlternateViews.Add(alternateView);

            //To - collection or one email
            foreach (var recipient in recipients.Split(','))
            {
                if (!string.IsNullOrEmpty(recipient))
                {
                    smtpMessage.To.Add(recipient);
                }
            }

            //From
            smtpMessage.From = new MailAddress(fromAddress);
            //Subject
            smtpMessage.Subject = subject;
            //Body
            smtpMessage.Body = textContent;

            //Sending
            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Send(smtpMessage);
                    newEmail.SetValue("emailSent", true);
                    newEmail.SetValue("emailSentDate", DateTime.Now);
                    _contentService.SaveAndPublish(newEmail);
                }
                catch (Exception exc)
                {
                    //Log the error
                    _logger.Error<EmailService>("Problem sending the email", exc);
                    throw exc;
                }
            }
        }

        // returns email template as IPublished content where title matches the template name. 
        private IPublishedContent GetEmailTemplate(string templateName)
        {
            var template = _umbraco.ContentAtRoot().DescendantsOrSelfOfType("emailTemplate").Where(w => w.Name == templateName).FirstOrDefault();
            return template;
        }

        /// <summary>
        /// Send a email link to th new member
        /// </summary>
        /// <param name="membersEmail"></param>
        /// <param name="verificationToken"></param>
        void IEmailService.SendVerifyEmailAddressNotification(string membersEmail, string verificationToken)
        {
            // Get Template
            var emailTemplate = GetEmailTemplate("Verify Email");

            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }
            // Fields from the template
            //Get the template data
            var subject = emailTemplate.Value<string>("emailTemplateSubjectLine");
            var htmlContent = emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");

            // Mail merge
            // build the url to be absolute url to the verify page.
            var url = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath, string.Empty);
            url += $"/verify?token={verificationToken}";

            MailMerge("verify-url", url, ref htmlContent, ref textContent);
            // Log the email & Send the email
            SendMail(membersEmail, subject, htmlContent, textContent);
        }

        private void MailMerge(string token, string value, ref string htmlContent, ref string textContent)
        {
            htmlContent = htmlContent.Replace($"##{token}##", value);
            textContent = textContent.Replace($"##{token}##", value);
        }
        /// <summary>
        /// Send Email link to the user
        /// </summary>
        /// <param name="membersEmail"></param>
        /// <param name="resetToken"></param>
        public void SendResetPasswordNotification(string membersEmail, string resetToken)
        {
            // Get Template
            var emailTemplate = GetEmailTemplate("Reset Password");

            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }
            
            // Get the data
            
            // Fields from the template
            //Get the template data
            var subject = emailTemplate.Value<string>("emailTemplateSubjectLine");
            var htmlContent = emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");
            
            // Mail merge
            // build the url to be absolute url to the verify page.
            var url = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath, string.Empty);
            url += $"/reset-password?token={resetToken}";
            
            // ##reset-url##
            MailMerge("reset-url", url, ref htmlContent, ref textContent);
            
            // Send
            SendMail(membersEmail, subject, htmlContent, textContent);
        }
        /// <summary>
        /// Send a note to the user telling them they changed there password
        /// </summary>
        /// <param name="membersEmail"></param>
        public void SendPasswordChangedNotification(string membersEmail)
        {
            // get template
            var emailTemplate = GetEmailTemplate("Password Changed");

            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }
            // Get the data out of the template
            var subject = emailTemplate.Value<string>("emailTemplateSubjectLine");
            var htmlContent = emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");

            // Send
            SendMail(membersEmail, subject, htmlContent, textContent);
        }
    }
}
