using HighlyDeveloped.Core.Interfaces;
using HighlyDeveloped.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace HighlyDeveloped.Core
{
    public class RegisterServicesComposer: IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IEmailService, EmailService>(Lifetime.Request);
        }
    }
}
