using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Onyx.Authorization
{
    /// <summary>
    /// Configuration settings for the Web API
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers the routes the Web API responds to
        /// </summary>
        /// <param name="config">Current configuration of the server element</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //enableCors doesn't affect owin why? 
            //var cors = new EnableCorsAttribute("http://onyximports.com.br,https://onyximports.com.br", "*", "*");
            //config.EnableCors(cors);
            
            // Allows us to map routes using [Route()] and [RoutePrefix()]
            config.MapHttpAttributeRoutes();

            
            // Default settings to handle routes that aren't explicitly named
            // through attributes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
