
using Microsoft.AspNetCore.Builder;
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
