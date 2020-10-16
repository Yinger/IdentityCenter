namespace RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel
{
    //---------------------------------------------------------------
    /// <summary>
    /// ロールの検索条件クラスです。
    /// </summary>
    //---------------------------------------------------------------
    public class RoleSearchModel
    {
        /// <summary>ロール名</summary>
        public string RoleName { get; set; }

        /// <summary>分類用のタグ</summary>
        public string Tag { get; set; }

        /// <summary>説明</summary>
        public string Description { get; set; }
    }
}