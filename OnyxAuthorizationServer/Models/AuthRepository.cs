using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using TwoFactorAuthentication.API.Services;
using System.Text.RegularExpressions;



namespace Onyx.Authorization.Models
{
    public class AuthRepository : IDisposable
    {
        private AuthorizationDb _ctx;
        private UserManager<ApplicationUser> _userManager;
        public AuthRepository()
        {
            _ctx = new AuthorizationDb();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }
        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                TwoFactorEnabled = true,
                PSK = TimeSensitivePassCode.GeneratePresharedKey()
            };
            var result = await _userManager.CreateAsync(user, userModel.Password);
            return result;
        }
        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = null;
            user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public static bool EmailValidate(string email)
        {
            if (string.IsNullOrEmpty(email)) { return false; }
            Regex a = new Regex("^[a-z0-9._-]+@([a-z0-9][a-z0-9-]*[a-z0-9].)+([a-z]+.)?([a-z]+)$", RegexOptions.IgnoreCase);
            return a.IsMatch(email);
        }
        public async Task<ApplicationUser> FindbyNameOrEmail(string UserNameOrEmail)
        {
            ApplicationUser user = null;

            if (EmailValidate(UserNameOrEmail))
            {
                user = await _userManager.FindByEmailAsync(UserNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(UserNameOrEmail);
            }
            return user;
        }
        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);
            return client;
        }
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();
            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }
            _ctx.RefreshTokens.Add(token);
            return await _ctx.SaveChangesAsync() > 0;
        }
        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);
            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }
            return false;
        }
        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
            return await _ctx.SaveChangesAsync() > 0;
        }
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);
            return refreshToken;
        }
        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }
        public async Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo)
        {
            ApplicationUser user = await _userManager.FindAsync(loginInfo);
            return user;
        }
        public async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            return result;
        }
        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);
            return result;
        }


        public async Task<List<Claim>> GetRoleClaims(string userId)
        {
            var roleClaims =
                      from role in await _userManager.GetRolesAsync(userId)
                      select new Claim(ClaimTypes.Role, role);
            return roleClaims.ToList();
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }

}


