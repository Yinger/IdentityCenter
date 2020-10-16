using System;

namespace RcrsCore.IdentityServer.Models.Identity
{
    //---------------------------------------------------------------
    /// <summary>
    /// アカウント関数
    /// </summary>
    //---------------------------------------------------------------
    public class AccountOptions
    {
        /// <summary></summary>
        public static bool AllowLocalLogin = true;

        /// <summary></summary>
        public static bool AllowRememberLogin = true;

        /// <summary></summary>
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        /// <summary></summary>
        public static bool ShowLogoutPrompt = true;

        /// <summary>元々の項目へ自動Redirect</summary>
        public static bool AutomaticRedirectAfterSignOut = true;

        /// <summary>specify the Windows authentication scheme being used</summary>
        public static readonly string WindowsAuthenticationSchemeName = Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;

        /// <summary>if user uses windows auth, should we load the groups from windows</summary>
        public static bool IncludeWindowsGroups = false;

        /// <summary></summary>
        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}