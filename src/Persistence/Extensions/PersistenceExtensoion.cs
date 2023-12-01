using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Infrastructure;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Services;

namespace Persistence.Extensions
{
    public static class PersistenceExtensoion
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            //  services.AddScoped<ITokenService, TokenService>();

            // using var scope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
            // var context = services.BuildServiceProvider().GetRequiredService<ApplicationContext>();
            // var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            // var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            // SeedData.EnsureSeedData(context, userMgr, roleMgr);
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}