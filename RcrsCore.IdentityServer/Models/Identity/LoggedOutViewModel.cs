namespace RcrsCore.IdentityServer.Models.Identity
{
    //---------------------------------------------------------------
    /// <summary>
    /// Logout後表示用のモデル
    /// </summary>
    //---------------------------------------------------------------
    public class LoggedOutViewModel
    {
        /// <summary></summary>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary></summary>
        public string ClientName { get; set; }

        /// <summary></summary>
        public string SignOutIframeUrl { get; set; }

        /// <summary></summary>
        public bool AutomaticRedirectAfterSignOut { get; set; } = false;

        /// <summary></summary>
        public string LogoutId { get; set; }

        /// <summary></summary>
        public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;

        /// <summary></summary>
        public string ExternalAuthenticationScheme { get; set; }
    }
}