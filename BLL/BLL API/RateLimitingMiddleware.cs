﻿using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreRateLimit;

namespace API_Maestros_Core.BLL
{
    public static class RateLimitingMiddleware
    {
       
        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            // Used to store rate limit counters and ip rules
            services.AddMemoryCache();

            // Load in general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(options => configuration.GetSection("IpRateLimitingSettings").Bind(options));

            // Inject Counter and Store Rules
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddInMemoryRateLimiting();

            // Return the services
            return services;
        }

        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
            return app;
        }
    }
}
