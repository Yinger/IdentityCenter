using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RcrsCore.IdentityServer.Data.SeedData;

namespace RcrsCore.IdentityServer
{
    //---------------------------------------------------------------
    /// <summary>
    /// システムはここから始まります。
    /// </summary>
    //---------------------------------------------------------------
    public class Program
    {
        //---------------------------------------------------------------
        /// <summary>
        /// 開始方法
        /// </summary>
        /// <param name="args"></param>
        //---------------------------------------------------------------
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // DB初期化またテスト用データを自動入力します。
            Seed.EnsureSeedData(host.Services);

            Console.WriteLine("起動成功！，Port：5002");
            Console.WriteLine();

            host.Run();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Configureします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}