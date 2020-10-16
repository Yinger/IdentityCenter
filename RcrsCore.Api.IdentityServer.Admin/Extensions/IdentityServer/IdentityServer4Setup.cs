using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RcrsCore.Api.IdentityServer.Admin.Helper;

namespace RcrsCore.Api.IdentityServer.Admin.Extensions.IdentityServer
{
    //-------------------------------------------------------------------------------
    /// <summary>
    /// IdentityServer4に関係の設定クラスです。
    /// </summary>
    //-------------------------------------------------------------------------------
    public static class IdentityServer4Setup
    {
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Setup by Startup
        /// </summary>
        /// <param name="services"></param>
        //-------------------------------------------------------------------------------
        public static void AddIdentityServerDbContext(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            string connectionString = AppSettingsHelper.App(new string[] { "ConnectionStrings", "DefaultConnection" });

            // adding and configuring IdentityServer
            var builder = services.AddIdentityServer()
                // this adds the config data from DB (clients,resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlite(connectionString);
                })
                // this adds the operational data from DB (code, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlite(connectionString);
                });
        }
    }
}