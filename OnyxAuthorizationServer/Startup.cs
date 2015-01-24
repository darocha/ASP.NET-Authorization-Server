using System.Web.Http;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.OAuth;
using System;
using Onyx.Authorization.Formats;
using Onyx.Authorization.Providers;
using System.Web.Cors;
using System.Threading.Tasks;

/// This assembly attribute directs Microsoft.Owin to use the Startup class
/// defined in this file as the start of our application
[assembly: OwinStartup(typeof(Onyx.Authorization.Startup))]

namespace Onyx.Authorization
{
    /// <summary>
    /// Startup class used by OWIN implementations to run the Web application
    /// </summary>
    public partial class Startup
    {
       
        /// <summary>
        /// Used to create an instance of the Web application 
        /// </summary>
        /// <param name="app">Parameter supplied by OWIN implementation which our configuration is connected to</param>
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            var policy = new CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                AllowAnyOrigin = false,
                SupportsCredentials = true
                
            };

            policy.Origins.Add("http://onyximports.com.br");
            policy.Origins.Add("https://onyximports.com.br");

            //add custom access headers
            //policy.ExposedHeaders.Add("X-Custom-Header");
            //app.UseCors(CorsOptions.AllowAll);
            app.UseCors(new CorsOptions { PolicyProvider = new CorsPolicyProvider { PolicyResolver = context => Task.FromResult(policy) } });

            // Wire-in the authentication middleware
            ConfigureAuth(app);

            // Handles registration of the Web API's routes
            WebApiConfig.Register(config);

            // Add the Web API framework to the app's pipeline
            app.UseWebApi(config);
        }


    }
}