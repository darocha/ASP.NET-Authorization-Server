using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Threading;
using System.IdentityModel.Tokens;
using System.IdentityModel.Services;

namespace Onyx.Authorization
{
    public class CustomAuthenticationManager : ClaimsAuthenticationManager
    {

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return base.Authenticate(resourceName, incomingPrincipal);
            }

            ClaimsPrincipal transformedPrincipal = TransformPrincipal(incomingPrincipal);

            CreateSession(transformedPrincipal);

            return transformedPrincipal;
        }
        private void CreateSession(ClaimsPrincipal transformedPrincipal)
        {
            SessionSecurityToken sessionSecurityToken = new SessionSecurityToken(transformedPrincipal, TimeSpan.FromHours(8));
            FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionSecurityToken);
        }
        private ClaimsPrincipal TransformPrincipal(ClaimsPrincipal incomingPrincipal)
        {
            String userName = incomingPrincipal.Identity.Name;

            ClaimsIdentity newIdentity = new ClaimsIdentity("Custom");

            newIdentity.AddClaims(incomingPrincipal.Claims);

            ClaimsPrincipal newPrincipal = new ClaimsPrincipal(newIdentity);

            return newPrincipal;
            /*
            List<Claim> claims = new List<Claim>();

            //simulate database lookup
            if (userName.IndexOf("andras", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                claims.Add(new Claim(ClaimTypes.Country, "Sweden"));
                claims.Add(new Claim(ClaimTypes.GivenName, "Andras"));
                claims.Add(new Claim(ClaimTypes.Name, "Andras"));
                claims.Add(new Claim(ClaimTypes.Role, "IT"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.GivenName, userName));
                claims.Add(new Claim(ClaimTypes.Name, userName));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));*/
        }
        /*
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return base.Authenticate(resourceName, incomingPrincipal);
            }
            return TransformPrincipal(incomingPrincipal);
        }

        private ClaimsPrincipal TransformPrincipal(ClaimsPrincipal incomingPrincipal)
        {
            //var manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            
            // this breakpoint is hit last
            ClaimsIdentity newIdentity = new ClaimsIdentity("Custom");
            newIdentity.AddClaims(incomingPrincipal.Claims);
            
            // I add some additional claims
            ClaimsPrincipal newPrincipal = new ClaimsPrincipal(newIdentity);
            return newPrincipal;
        }
        */


    }
}