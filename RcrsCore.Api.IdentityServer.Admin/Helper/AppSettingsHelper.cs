using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace RcrsCore.Api.IdentityServer.Admin.Helper
{
    //-------------------------------------------------------------------------------
    /// <summary>
    /// appsettings.json の操作を簡単化
    /// </summary>
    //-------------------------------------------------------------------------------
    public class AppSettingsHelper
    {
        private static IConfiguration Configuration { get; set; }
        private readonly string path = "appsettings.json";

        //-------------------------------------------------------------------------------
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="contentPath"></param>
        //-------------------------------------------------------------------------------
        public AppSettingsHelper(string contentPath)
        {
            Configuration = new ConfigurationBuilder()
               .SetBasePath(contentPath)
               .Add(new JsonConfigurationSource { Path = path, Optional = false, ReloadOnChange = true }) ////bin下のappsettings.json変更支持
               .Build();
        }

        ////-------------------------------------------------------------------------------
        ///// <summary>
        ///// 初期化
        ///// </summary>
        ///// <param name="configuration"></param>
        ////-------------------------------------------------------------------------------
        //public AppsettingsHelper(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //-------------------------------------------------------------------------------
        /// <summary>
        /// 設定を取得します。
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        //-------------------------------------------------------------------------------
        public static string App(params string[] sections)
        {
            try
            {
                if (sections.Any())
                    return Configuration[string.Join(":", sections)];
            }
            catch (Exception) { }

            return "";
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// 設定を一括取得します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        //-------------------------------------------------------------------------------
        public static List<T> App<T>(params string[] sections)
        {
            List<T> list = new List<T>();

            Configuration.Bind(string.Join(":", sections), list);

            return list;
        }
    }
}