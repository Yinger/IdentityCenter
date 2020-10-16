using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.IdentityServer.Extensions.IdentityServer;
using RcrsCore.IdentityServer.Helper;
using RcrsCore.IdentityServer.Models;

namespace RcrsCore.IdentityServer
{
    //---------------------------------------------------------------
    /// <summary>
    /// add-migration InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer
    /// add-migration InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer
    /// add-migration AppDbMigration -c ApplicationDbContext -o Data/Migrations/Application
    /// add-migration CityDbMigration -c CityDbContext -o Data/Migrations/City
    ///
    /// update-database -c PersistedGrantDbContext
    /// update-database -c ConfigurationDbContext
    /// update-database -c ApplicationDbContext
    /// update-database -c CityDbContext
    ///
    /// *** Remove-Migration -c ApplicationDbContext *** ←削除用
    /// </summary>
    //---------------------------------------------------------------
    public class Startup
    {
        /// <summary> Configuration </summary>
        public IConfiguration Configuration { get; }

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="configuration"></param>
        //---------------------------------------------------------------
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// This method gets called by the runtime.
        /// Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        //---------------------------------------------------------------
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            services.AddSingleton(new AppSettingsHelper(basePath));

            // DBを設定します。
            string connectionString = AppSettingsHelper.App(new string[] { "ConnectionStrings", "DefaultConnection" });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
            services.AddDbContext<CityDbContext>(options => options.UseSqlite(connectionString));

            // creating and configuring Microsoft.AspNetCore.Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/oauth2/authorize");
            });

            // setting up authentication
            services.AddAuthentication();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            //IdentityServer
            services.AddIdentityServer4Setup();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// This method gets called by the runtime.
        /// Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        //---------------------------------------------------------------
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            //IdentityServe(※UseRouting後に実行)
            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}