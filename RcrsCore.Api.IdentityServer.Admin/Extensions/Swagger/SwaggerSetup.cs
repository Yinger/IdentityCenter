using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RcrsCore.IdentityServer.Dto;
using RcrsCore.Api.IdentityServer.Admin.Helper;

namespace RcrsCore.Api.IdentityServer.Admin.Extensions.Swagger
{
    //-------------------------------------------------------------------------------
    /// <summary>
    /// Swaggerをsetupします。
    /// </summary>
    //-------------------------------------------------------------------------------
    public static class SwaggerSetup
    {
        //-------------------------------------------------------------------------------
        /// <summary>
        /// setup
        /// </summary>
        /// <param name="services"></param>
        //-------------------------------------------------------------------------------
        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var basePath = AppContext.BaseDirectory;
            string connectionString = AppSettingsHelper.App(new string[] { "ConnectionStrings", "DefaultConnection" });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = $"{IdentityConst.ApiName} ドキュメント - Netcore 3.1",
                    Description = $"{IdentityConst.ApiName} HTTP API - " + connectionString
                });

                //mulitVersion対応
                //typeof(ApiVersion.ApiVersions).GetEnumNames().ToList().ForEach(version =>
                //{
                //    c.SwaggerDoc(version, new OpenApiInfo
                //    {
                //        Version = version,
                //        Title = $"{ApiName} ドキュメント - Netcore 3.1",
                //        Description = $"{ApiName} HTTP API " + version
                //    });
                //    c.OrderActionsBy(o => o.RelativePath);
                //});

                try
                {
                    var xmlPath = Path.Combine(basePath, "RcrsCore.Api.IdentityServer.Admin.xml");
                    c.IncludeXmlComments(xmlPath, true);

                    var xmlModelPath = Path.Combine(basePath, "RcrsCore.Api.IdentityServer.Admin.Dto.xml");
                    c.IncludeXmlComments(xmlModelPath);
                }
                catch
                {
                    Console.WriteLine("RcrsCore.Api.IdentityServer.Admin.xml or RcrsCore.Api.IdentityServer.Admin not found");
                }
            });
        }
    }
}