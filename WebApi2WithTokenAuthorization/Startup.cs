using System;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApi2WithTokenAuthorization.Providers;

[assembly: OwinStartup(typeof(WebApi2WithTokenAuthorization.Startup))]
namespace WebApi2WithTokenAuthorization
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }
        public static MicrosoftAccountAuthenticationOptions microsoftAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            log4net.Config.XmlConfigurator.Configure();

        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);

            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            var options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(30),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            //Configure Google External Login
            googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "906041197601-v5v95tijskeiltb1bj2kbur9m0tha1pf.apps.googleusercontent.com",
                ClientSecret = "FVmI7WRUV1bF0TOazSk_2I1n",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(googleAuthOptions);

            //Configure Facebook External Login
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = "1556891417855856",
                AppSecret = "363f6b4377706c270b7ec6afaae23525",
                Provider = new FacebookAuthProvider(),
            };
            facebookAuthOptions.Scope.Add("email");
            app.UseFacebookAuthentication(facebookAuthOptions);

            //configure Microsoft external login
            microsoftAuthOptions = new MicrosoftAccountAuthenticationOptions
            {
                ClientId = "0000000048131E21",
                ClientSecret = "r-I7BrnlogBFQGc2BlAfFQeYMRMHLF9r", 
                Provider = new MicrosoftAccountAuthenticationProvider()
            };
            app.UseMicrosoftAccountAuthentication(microsoftAuthOptions);

        }
    }
}