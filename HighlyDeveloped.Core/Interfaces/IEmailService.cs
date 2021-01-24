using HighlyDeveloped.Core.ViewModel;
using System;

namespace HighlyDeveloped.Core.Interfaces
{
    // Handles all email Services
    public interface IEmailService
    {
        void SendContactNotificationToAdmin(ContactFormViewModel vm);
        void SendVerifyEmailAddressNotification(String membersEmail, string verificationToken);
        void SendResetPasswordNotification(string membersEmail, string resetToken);
        void SendPasswordChangedNotification(string membersEmail);
    }
}
