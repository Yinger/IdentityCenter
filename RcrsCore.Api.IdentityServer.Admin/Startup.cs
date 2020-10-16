using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RcrsCore.IdentityServer.Dto;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.Api.IdentityServer.Admin.Extensions.IdentityServer;
using RcrsCore.Api.IdentityServer.Admin.Extensions.Swagger;
using RcrsCore.Api.IdentityServer.Admin.Helper;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.City;

namespace RcrsCore.Api.IdentityServer.Admin
{
    //---------------------------------------------------------------
    /// <summary>
    /// Scaffold-DbContext "data source=identity.sqlite3" Microsoft.EntityFrameworkCore.Sqlite -OutputDir Models/DbFirst/City/Entity -Tables "M_都道府県","M_市区町村" -ContextDir Models/DbFirst/City -Context CityContext
    /// Scaffold-DbContext "data source=identity.sqlite3" Microsoft.EntityFrameworkCore.Sqlite -OutputDir Models/DbFirst/Application/Entity -Tables "AspNetRoleClaims","AspNetUserClaims","AspNetUserLogins","AspNetUserTokens","M_Role","M_User","T_UserRole" -ContextDir Models/DbFirst/Application -Context ApplicationContext
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
            services.AddControllers();

            //AppSettingsHelper
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            services.AddSingleton(new AppSettingsHelper(basePath));

            //DB
            string connectionString = AppSettingsHelper.App(new string[] { "ConnectionStrings", "DefaultConnection" });
            services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connectionString));
            services.AddDbContext<CityContext>(options => options.UseSqlite(connectionString));
            services.AddIdentityServerDbContext();

            // creating and configuring Microsoft.AspNetCore.Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationContext>();

            //Swagger
            services.AddSwaggerSetup();
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

            //Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/V1/swagger.json", $"{IdentityConst.ApiName} V1");

                c.RoutePrefix = "";
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}