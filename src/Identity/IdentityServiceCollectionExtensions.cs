using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Everyday.b.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            //var userType = typeof(UserStore)
            services.AddScoped<IUserStore, UserStore>();
            services.AddScoped<UserManager, UserManager>();
            services.AddScoped<ITokenProvider,JwtTokenProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }
    }
}
