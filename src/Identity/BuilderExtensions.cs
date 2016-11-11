using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Csys.Identity
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseIdentity(this IApplicationBuilder app, TokenValidationParameters tokenValidationParameters)
        {
            
            var jwtBearerOptions = new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            };
            app.UseJwtBearerAuthentication(jwtBearerOptions);
            return app;
        }
    }

}
