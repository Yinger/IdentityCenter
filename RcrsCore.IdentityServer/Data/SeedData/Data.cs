using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.IdentityServer.Dto.DomainModel.City;

namespace RcrsCore.IdentityServer.Data.SeedData
{
    //---------------------------------------------------------------
    /// <summary>
    ///
    /// </summary>
    //---------------------------------------------------------------
    public class Data
    {
        //---------------------------------------------------------------
        /// <summary>
        /// scopes define the resources in your system
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(JwtClaimTypes.Role, "ロール", new List<string> { JwtClaimTypes.Role }),
                //new IdentityResource(IdentityConst.JwtClaimTypes.RoleName, "ロール名", new List<string> { "rolename" }),
                new IdentityResource(IdentityConst.CustomJwtClaimTypes.LgCode, "lgcode", new List<string> { "lgcode" })
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {
                new ApiResource("rcrs.core.api", "Rcrs.Core API") {
                    // include the following using claims in access token (in addition to subject id)
                    // requires using using IdentityModel;
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Role , "lgcode", "rolename","userid"},
                    ApiSecrets = new List<Secret>()
                    {
                        new Secret("api_secret".Sha256())
                    },
                }
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// clients want to access resources (aka scopes)
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                //============================================== MVC Client ==============================================
                new Client
                {
                    ClientId = "RcrsCore.MVC",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,//true時、同意画面を表示します。
                    RequirePkce = true,//認証コードベースのトークンに証明キーが必要かどうかを指定します。
                    AlwaysIncludeUserClaimsInIdToken=true, //すべてのclaimsをIdTokenに保存のことです。

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:10241/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:10241/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityConst.Scopes.LgCode,
                        IdentityConst.Scopes.Role,
                        //IdentityConst.Scopes.RoleName,
                        IdentityConst.Scopes.UserId
                    }
                }
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public static IEnumerable<ApplicationRole> GetRoles()
        {
            return new List<ApplicationRole>
            {
                new ApplicationRole
                {
                    Name = "tmp",
                    TagCD = "1",
                    Description = "tmp role"
                },
                new ApplicationRole
                {
                    Name = "kanzai",
                    TagCD = "2",
                    Description = "kanzai role"
                }
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public static IEnumerable<ApplicationUser> GetUsers()
        {
            return new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "rcrs-dev-T",
                    Email = "rcrs-dev-team@chklab.com",
                    LgCode = "300000",
                    LgKaKakari = "技術",
                    IsAdmin = true,
                    IsChiiki = true,
                    EmailConfirmed = false
                }
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 都道府県
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public static IEnumerable<M_都道府県> GetKans()
        {
            return new List<M_都道府県>
            {
                new M_都道府県
                {
                    都道府県CD = "30",
                    都道府県名 = "○○県"
                },
                new M_都道府県
                {
                    都道府県CD = "31",
                    都道府県名 = "□□県"
                }
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public static IEnumerable<M_市区町村> GetCity()
        {
            return new List<M_市区町村>
            {
                new M_市区町村
                {
                    都道府県CD = "30",
                    市区町村CD = "300000",
                    都道府県名 = "○○県",
                    市区町村名 = "○○市"
                },
                new M_市区町村
                {
                    都道府県CD = "30",
                    市区町村CD = "300001",
                    都道府県名 = "○○県",
                    市区町村名 = "○○○市"
                },
                new M_市区町村
                {
                    都道府県CD = "30",
                    市区町村CD = "300002",
                    都道府県名 = "○○県",
                    市区町村名 = "○○○○市"
                },
                new M_市区町村
                {
                    都道府県CD = "31",
                    市区町村CD = "400000",
                    都道府県名 = "□□県",
                    市区町村名 = "□□市"
                },
                new M_市区町村
                {
                    都道府県CD = "31",
                    市区町村CD = "400001",
                    都道府県名 = "□□県",
                    市区町村名 = "□□□市"
                },
                new M_市区町村
                {
                    都道府県CD = "31",
                    市区町村CD = "400002",
                    都道府県名 = "□□県",
                    市区町村名 = "□□□□市"
                }
            };
        }
    }
}