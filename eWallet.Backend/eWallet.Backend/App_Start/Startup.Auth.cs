using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace eWallet.Backend
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");
            Microsoft.Owin.Security.Google.GoogleOAuth2AuthenticationOptions option = new Microsoft.Owin.Security.Google.GoogleOAuth2AuthenticationOptions();
            option.CallbackPath = new PathString("/signin-google");
            option.ClientId = System.Configuration.ConfigurationSettings.AppSettings["GoogleClientKey"];
            option.ClientSecret = System.Configuration.ConfigurationSettings.AppSettings["GoogleClientSecret"];
            
            app.UseGoogleAuthentication(option);
        }
    }
}