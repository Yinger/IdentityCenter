using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RcrsCore.IdentityServer.Models;
using RcrsCore.IdentityServer.Models.DomainEntity;

namespace RcrsCore.IdentityServer.Data.SeedData
{
    //---------------------------------------------------------------
    /// <summary>
    ///
    /// </summary>
    //---------------------------------------------------------------
    public class Seed
    {
        //---------------------------------------------------------------
        /// <summary>
        /// テスト用データを自動生成します。
        /// </summary>
        /// <param name="serviceProvider"></param>
        //---------------------------------------------------------------
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding database...");

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                // seed CityDb
                var cityDbContext = scope.ServiceProvider.GetRequiredService<CityDbContext>();
                cityDbContext.Database.Migrate();
                ensureSeedData(cityDbContext);

                // seed PersistedGrantDb
                var persistedGrantContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                persistedGrantContext.Database.Migrate();
                ensureSeedData(persistedGrantContext);

                // seed ApplicationDb
                var applicationContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                applicationContext.Database.Migrate();
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var users = Data.GetUsers();
                var roles = Data.GetRoles();

                foreach (var role in roles)
                {
                    if (role == null || string.IsNullOrEmpty(role.Name))
                        continue;

                    var roleInDB = roleMgr.FindByNameAsync(role.Name).Result;

                    if (roleInDB == null)
                    {
                        var result = roleMgr.CreateAsync(role).Result;

                        if (!result.Succeeded)
                            throw new Exception(result.Errors.First().Description);

                        Console.WriteLine($"{role?.Name} created");//AspNetUserClaims テーブル
                    }
                }

                foreach (var user in users)
                {
                    if (user == null || string.IsNullOrEmpty(user.UserName))
                        continue;

                    var userInDB = userMgr.FindByNameAsync(user.UserName).Result;
                    var userRoleId = roleMgr.Roles.FirstOrDefault()?.Id;
                    var userRoleList = roleMgr.Roles.ToList();
                    var roleName = userRoleList.First().Name;

                    if (userInDB == null)
                    {
                        if (userRoleId != null && userRoleList.Count > 0)
                        {
                            var result = userMgr.CreateAsync(user, IdentityConst.DefaultPassword).Result;

                            if (!result.Succeeded)
                                throw new Exception(result.Errors.First().Description);

                            var claims = new List<Claim>{
                                    new Claim(JwtClaimTypes.Name, user.UserName),
                                    new Claim(JwtClaimTypes.Email, user.Email),
                                    //new Claim(IdentityConst.JwtClaimTypes.RoleName, roleName),
                                    new Claim(IdentityConst.CustomJwtClaimTypes.LgCode, user.LgCode),
                                    new Claim(IdentityConst.CustomJwtClaimTypes.UserId, user.Id.ToString().ToUpper()),
                                };

                            claims.AddRange(userRoleList.Select(s => new Claim(JwtClaimTypes.Role, s.ToString())));
                            result = userMgr.AddClaimsAsync(user, claims).Result;

                            if (!result.Succeeded)
                                throw new Exception(result.Errors.First().Description);
                            Console.WriteLine($"{user?.UserName} created");//AspNetUserClaims テーブル

                            //AspNetUserRoles
                            foreach (ApplicationRole role in userRoleList)
                                applicationContext.UserRoles.Add(new ApplicationUserRole { UserId = user.Id, RoleId = role.Id });
                            applicationContext.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine($"{user?.UserName} doesn't have a corresponding role.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{user?.UserName} already exists");
                    }
                }
            }

            Console.WriteLine("Done seeding database.");
            Console.WriteLine();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// scopes、api、client seed します。
        /// </summary>
        /// <param name="context"></param>
        //---------------------------------------------------------------
        private static void ensureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Console.WriteLine("Clients being populated");

                foreach (var client in Data.GetClients().ToList())
                    context.Clients.Add(client.ToEntity());

                context.SaveChanges();
            }
            else
                Console.WriteLine("Clients already populated");

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("IdentityResources being populated");

                foreach (var resource in Data.GetIdentityResources().ToList())
                    context.IdentityResources.Add(resource.ToEntity());

                context.SaveChanges();
            }
            else
                Console.WriteLine("IdentityResources already populated");

            if (!context.ApiResources.Any())
            {
                Console.WriteLine("ApiResources being populated");

                foreach (var resource in Data.GetApiResources().ToList())
                    context.ApiResources.Add(resource.ToEntity());

                context.SaveChanges();
            }
            else
                Console.WriteLine("ApiResources already populated");
        }

        //---------------------------------------------------------------
        /// <summary>
        /// M_都道府県、M_市区町村を初期化します。
        /// </summary>
        /// <param name="context"></param>
        //---------------------------------------------------------------
        private static void ensureSeedData(CityDbContext context)
        {
            if (!context.M_都道府県s.Any())
            {
                Console.WriteLine("M_都道府県 being populated");

                foreach (var kan in Data.GetKans().ToList())
                    context.M_都道府県s.Add(kan);

                context.SaveChanges();
            }
            else
                Console.WriteLine("M_都道府県 already populated");

            if (!context.M_市区町村s.Any())
            {
                Console.WriteLine("M_市区町村 being populated");

                foreach (var city in Data.GetCity().ToList())
                    context.M_市区町村s.Add(city);

                context.SaveChanges();
            }
            else
                Console.WriteLine("M_市区町村 already populated");
        }
    }
}