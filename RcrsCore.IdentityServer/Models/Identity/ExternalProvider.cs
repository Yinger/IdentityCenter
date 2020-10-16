namespace RcrsCore.IdentityServer.Models.Identity
{
    //---------------------------------------------------------------
    /// <summary>
    /// 外部認証Provider（ex:Google，Facebook，Twitter，Microsoft Account）
    /// </summary>
    //---------------------------------------------------------------
    public class ExternalProvider
    {
        /// <summary></summary>
        public string DisplayName { get; set; }

        /// <summary></summary>
        public string AuthenticationScheme { get; set; }
    }
}