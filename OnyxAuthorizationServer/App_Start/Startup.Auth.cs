using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Onyx.Authorization.Models;
using Onyx.Authorization.Providers;
using Owin;
using Onyx.Authorization.Formats;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using System.Web.Helpers;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.Owin;

namespace Onyx.Authorization
{
    /// <summary>
    /// Partial of the class used to start the web application
    /// </summary>
    public partial class Startup
    {

        public static string PublicClientId { get; private set; }

        public static OAuthAuthorizationServerOptions OAuthJwtOptions { get; private set; }
        /// <summary>
        /// Options for the authorization system to use when authenticating users
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        /// <summary>
        /// Factory function that returns a new instance of the UserManager
        /// </summary>
        public static Func<UserManager<IdentityUser>> UserManagerFactory { get; set; }

        /// <summary>
        /// Static constructor to initialize values on application runtime
        /// </summary>
        static Startup()
        {
            // The "service" (our application) certifying a user's authentication status
            // String PublicClientId = "self";
            
            // Sets the UserManagerFactory to an anonymous function that returns a new
            // instance of UserManager<IdentityUser>. This factory can be called from
            // anywhere in the application as Startup.UserManagerFactory() to get a properly
            // configured instance of the UserManager
            UserManagerFactory = () => new UserManager<IdentityUser>(new UserStore<IdentityUser>(new AuthorizationDb()));

            // Options which the authentication system will use
            /*
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                // Point at which the Bearer token middleware will be mounted
                TokenEndpointPath = new PathString("/token"),
                // An implementation of the OAuthAuthorizationServerProvider which the middleware
                // will use for determining whether a user should be authenticated or not
                Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
                // How long a bearer token should be valid for
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(24),
                // Allows authentication over HTTP instead of forcing HTTPS
                AllowInsecureHttp = true
            };*/

            
        }

        /// <summary>
        /// Configures the application to use the OAuthBearerToken middleware
        /// </summary>
        /// <param name="app">The application to mount the middleware on</param>
        public void ConfigureAuth(IAppBuilder app)
        {

            AntiForgeryConfig.UniqueClaimTypeIdentifier = JwtRegisteredClaimNames.Sub;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            

            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(AuthorizationDb.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions() { 
            
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieDomain=".onyximports.com.br",
                CookieName = "OnyxApplicationCookie",
                Provider = new CookieAuthenticationProvider {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account. 
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
                

            });

            app.UseResourceAuthorization(new AuthorizationManager());

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

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

            //app.UseGoogleAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            OAuthJwtOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new SimpleAuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(1),
                AllowInsecureHttp = true,
                RefreshTokenProvider = new SimpleRefreshTokenProvider(),
                AccessTokenFormat = new CustomJwtFormat("http://authorization.onyximports.com.br")

            };

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            // Mounts the middleware on the provided app with the options configured
            // above
            app.UseOAuthBearerTokens(OAuthJwtOptions);
            //app.UseOAuthBearerTokens(OAuthOptions);

            //start - configuration validate and de-serialize JWT tokens
            var issuer = "http://authorization.onyximports.com.br";
            var audience = "ngAuthApp";
            var secret = TextEncodings.Base64Url.Decode("IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw");

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audience },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                    }
                });
            //ends - configuration validate and de-serialize JWT tokens

          


            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthJwtOptions);

           

        }

    }
}
