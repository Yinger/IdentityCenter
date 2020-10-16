namespace RcrsCore.IdentityServer.Models.Identity
{
    //---------------------------------------------------------------
    /// <summary>
    /// logout確認設定用
    /// </summary>
    //---------------------------------------------------------------
    public class LogoutViewModel : LogoutInputModel
    {
        /// <summary>trueの場合確認の画面が表示する</summary>
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}