using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Csys.Common;
using Csys.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Csys.Identity
{
    public class UserManager : IDisposable
    {
        private readonly IUserStore _store;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPasswordHasher _passwordHasher;
        private bool _disposed;

        public UserManager(IUserStore store,
            ITokenProvider tokenProvider,
            IPasswordHasher passwordHasher)
        {
            _store = store;
            _tokenProvider = tokenProvider;
            _passwordHasher = passwordHasher;
        }

        private readonly HttpContext _context;
        private CancellationToken CancellationToken => _context?.RequestAborted ?? CancellationToken.None;

        public async Task<TaskResult> CreateAsync(SignUpModel model)
        {
            var user = new User
            {
                Nickname = model.Nickname,
                Email = model.Email
            };
            var result = await ValidateUser(user);
            if (!result.Succeeded)
                return result;
            user.PasswordHash = _passwordHasher.HashPassword(model.Password);
            return await _store.CreateAsync(user, CancellationToken);
        }

        private static void UpdateSecurityStamp(User user)
        {
            user.SecurityStamp = Guid.NewGuid().ToString();
        }

        public async Task<TaskResult> UpdateSecurityStampAsync(User user)
        {
            UpdateSecurityStamp(user);
            return await UpdateUserAsync(user);
        }

        public async Task<TaskResult> UpdateUserAsync(User user)
        {
            var result = await ValidateUser(user);
            if (!result.Succeeded)
                return result;
            return await _store.UpdateAsync(user, CancellationToken);
        }

        public async Task<TaskResult> PasswordSignInAsync(string key, string password,
            string authenticationMethod = null)
        {
            ThrowIfDisposed();
            var user = await FindByEmailAsync(key);
            if (!CheckPassword(user, password)) return SignInResult.PwdNotCorrect;
            var result = await SignInAsync(user);
            return !result.Succeeded ? TaskResult.Failed(result.Errors) : SignInResult.Success(user);
        }

        public virtual bool CheckPassword(User user, string password)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(user.PasswordHash, password);

            if (result != PasswordVerificationResult.Success)
            {
                //Logger.LogWarning(0, "Invalid password for user {userId}.", await GetUserIdAsync(user));
                return false;
            }
            return true;
        }

        public async Task<TaskResult> SignInAsync(User user, string authenticationMethod = null)
        {
            UpdateSecurityStamp(user);
            UpdateToken(user);
            UpdateRefreshToken(user);
            return await UpdateUserAsync(user);
        }

        public async Task<TaskResult> TokenRefresh(string refreshToken)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _tokenProvider.ValidateRefreshToken(refreshToken);

            }
            catch (Exception)
            {
                return SignInResult.ValidateFailed;
            }

            var securityStamp = principal.Claims.Where(c => c.Type == "SecurityStamp")
                .Select(c => c.Value).FirstOrDefault();
            var id = principal.Identity.Name;
            var user = await FindByIdAsync(id);
            if (user.SecurityStamp != securityStamp) return SignInResult.ValidateFailed;
            if ((user.TokenExpires - DateTime.Now) < TimeSpan.FromDays(1))
            {
                DateTime expireTime;
                user.Token = _tokenProvider.GenerateToken(user, out expireTime);
                user.TokenExpires = expireTime;
            }
            return SignInResult.Success(user);
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await _store.FindByIdAsync(id, CancellationToken);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _store.FindByEmailAsync(email, CancellationToken);
        }

        private async Task<TaskResult> ValidateUser(User user)
        {
            //todo:添加注册校验
            return TaskResult.Success;
        }

        private void UpdateToken(User user)
        {
            DateTime expireTime;
            user.Token = _tokenProvider.GenerateToken(user, out expireTime);
            user.TokenExpires = expireTime;
        }
        public async Task<TaskResult> UpdateTokenAsync(User user)
        {
            UpdateToken(user);
            return await UpdateUserAsync(user);
        }
        private void UpdateRefreshToken(User user)
        {
            DateTime expireTime;
            user.RefreshToken = _tokenProvider.GenerateRefreshToken(user, out expireTime);
            user.RefreshTokenExpires = expireTime;
        }
        public async Task<TaskResult> UpdateRefreshTokenAsync(User user)
        {
            UpdateRefreshToken(user);
            return await UpdateUserAsync(user);
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;
            _store.Dispose();
            _disposed = true;
        }

    }
}
