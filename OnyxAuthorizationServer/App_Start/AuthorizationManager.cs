using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Owin;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityModel.Mvc;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace Onyx.Authorization
{
    [HandleForbidden]
    public class AuthorizationManager : ResourceAuthorizationManager
    {
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            context.Principal = ClaimsPrincipal.Current;

            var isAuthenticated = context.Principal.Identity.IsAuthenticated; 
            isAuthenticated = ClaimsPrincipal.Current.Identity.IsAuthenticated;

            if (!isAuthenticated)
            {
                return Nok();
            }

            var resource = context.Resource.First().Value;
            var claims = context.Principal.Claims.ToList();

            switch (resource)
            {
                case "SalesSummary": return Check(context);
                case "AccountManager": return context.Principal.HasClaim("access", "Administrator") ? Ok() : Check(context);
                case "RolesAdmin": return context.Principal.HasClaim("access", "Administrator") ? Ok() : Check(context);
                case "UsersAdmin": return context.Principal.HasClaim("access", "Administrator") ? Ok() : Check(context); 
                default: return Nok();
            }
        }

        private Task<bool> Check(ResourceAuthorizationContext context)
        {
            
            string action = context.Action.First().Value;

            switch (action)
            {
                case "Read": return Eval(context.Principal.HasClaim("access", "Administrator"));
                case "Write": return Eval(context.Principal.HasClaim("access", "Master"));

                case "ReadAccount": return Eval(context.Principal.HasClaim("access", "Master"));

                default: return Nok();
            }
        }
    }

}

