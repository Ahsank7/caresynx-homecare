using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Scheduler.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAppSwaggerUI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
            return app;
        }

        public static IApplicationBuilder UseAppCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors("AllowAllOrigins");
            return app;
        }
    }
} 