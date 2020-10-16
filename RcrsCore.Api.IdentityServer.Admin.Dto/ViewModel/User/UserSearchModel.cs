namespace RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel.User
{
    //---------------------------------------------------------------
    /// <summary>
    /// ユーザー検索条件モデル
    /// </summary>
    //---------------------------------------------------------------
    public class UserSearchModel
    {
        /// <summary>ユーザー名</summary>
        public string LoginName { get; set; }

        /// <summary>メールアドレス</summary>
        public string Email { get; set; }

        /// <summary>市区町村</summary>
        public string LgCode { get; set; }

        /// <summary>所属課</summary>
        public string LgKaKakari { get; set; }

        /// <summary>ロール名</summary>
        public string RoleName { get; set; }
    }
}