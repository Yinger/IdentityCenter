using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RcrsCore.IdentityServer.Data.SeedData;

namespace RcrsCore.IdentityServer
{
    //---------------------------------------------------------------
    /// <summary>
    /// �V�X�e���͂�������n�܂�܂��B
    /// </summary>
    //---------------------------------------------------------------
    public class Program
    {
        //---------------------------------------------------------------
        /// <summary>
        /// �J�n���@
        /// </summary>
        /// <param name="args"></param>
        //---------------------------------------------------------------
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // DB�������܂��e�X�g�p�f�[�^���������͂��܂��B
            Seed.EnsureSeedData(host.Services);

            Console.WriteLine("�N�������I�CPort�F5002");
            Console.WriteLine();

            host.Run();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Configure���܂��B
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