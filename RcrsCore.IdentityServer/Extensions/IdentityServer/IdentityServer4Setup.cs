using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.IdentityServer.Helper;

namespace RcrsCore.IdentityServer.Extensions.IdentityServer
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
        public static void AddIdentityServer4Setup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            string connectionString = AppSettingsHelper.App(new string[] { "ConnectionStrings", "DefaultConnection" });

            // adding and configuring IdentityServer
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                {
                    LoginUrl = "/oauth2/authorize",//Loginアドレス
                };
            })
                // use DB mode
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients,resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlite(connectionString,
                            sql => sql.MigrationsAssembly("RcrsCore.IdentityServer"));
                })
                // this adds the operational data from DB (code, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlite(connectionString,
                            sql => sql.MigrationsAssembly("RcrsCore.IdentityServer"));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                });

            // sets the temporary signing credential
            builder.AddDeveloperSigningCredential();
        }
    }
}