using HJPT.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace HJPT.Middlewares
{
    public class AnnounceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITrackService _trackService;
        private readonly TestBase test = new TestBase();
        public AnnounceMiddleware(RequestDelegate next, ITrackService trackService)
        {
            _next = next;
            _trackService = trackService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path != "/api/announce")
            {
                await _next(httpContext);
                return;
            }


            var result = await _trackService.Accept(httpContext.Request);

            httpContext.Response.StatusCode = 200;
            await httpContext.Response.WriteAsync(result);
            
        }

    }

    public static class AnnounceMiddlewareExtensions
    {
        public static IApplicationBuilder UseAnnounce(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AnnounceMiddleware>();
        }
    }
}
