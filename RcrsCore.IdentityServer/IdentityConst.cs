using System.Collections.Generic;

namespace RcrsCore.IdentityServer
{
    //---------------------------------------------------------------
    /// <summary>
    /// システムの定数クラスです。
    /// </summary>
    //---------------------------------------------------------------
    public static class IdentityConst
    {
        /// <summary></summary>
        public const string DefaultPassword = "PSwd@2020";

        /// <summary></summary>
        public const string ApiName = "identity api";

        //---------------------------------------------------------------
        /// <summary>
        /// Customize JwtClaimTypesの定数
        /// </summary>
        //---------------------------------------------------------------
        public static class CustomJwtClaimTypes
        {
            /// <summary></summary>
            public const string LgCode = "lgcode";

            /// <summary></summary>
            public const string UserId = "userid";

            /// <summary></summary>
            public const string Name = "name"; //JwtClaimTypes.Name

            /// <summary></summary>
            public const string Role = "role"; //JwtClaimTypes.role

            /// <summary></summary>
            public static List<string> DefaultClaimTypes = new List<string>
            {
                Name,
                Role,
                LgCode,
                UserId
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Customize Scopesの定数
        /// </summary>
        //---------------------------------------------------------------
        public static class Scopes
        {
            /// <summary></summary>
            public const string LgCode = "lgcode";

            /// <summary></summary>
            public const string Role = "role";

            /// <summary></summary>
            public const string RoleName = "rolename";

            /// <summary></summary>
            public const string UserId = "userid";

            /// <summary></summary>
            public const string OpenId = "openid"; //IdentityServerConstants.StandardScopes.OpenId

            /// <summary></summary>
            public const string Profile = "profile"; //IdentityServerConstants.StandardScopes.Profile

            /// <summary></summary>
            public const string Email = "email"; //IdentityServerConstants.StandardScopes.Email

            /// <summary></summary>
            public static List<string> ClientDefaultScopes = new List<string> {
                        OpenId,
                        Profile,
                        Email,
                        LgCode,
                        Role,
            };
        }
    }
}