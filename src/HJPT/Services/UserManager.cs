using HJPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HJPT.Services
{
    public class UserManager : UserManager<ApplicationUser>
    {
        public UserManager(IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<ApplicationUser>> logger)
            : base(store,
                  optionsAccessor,
                  passwordHasher, 
                  userValidators,
                  passwordValidators,
                  keyNormalizer, errors,
                  services, 
                  logger)
        {

        }

        public Task<ApplicationUser> FindUserByPassKey(string passKey)
        {
            var user = this.Users.FirstOrDefault(u => u.PassKey == passKey);
            if (user == null) throw new NullReferenceException();
            return Task.FromResult(user);
        }
    }
}
