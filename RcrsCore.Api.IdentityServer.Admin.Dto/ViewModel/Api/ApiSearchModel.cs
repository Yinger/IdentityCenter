namespace RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel.Api
{
    //---------------------------------------------------------------
    /// <summary>
    /// API検索条件モデル
    /// </summary>
    //---------------------------------------------------------------
    public class ApiSearchModel
    {
        /// <summary>表示名</summary>
        public string ApiName { get; set; }

        /// <summary>説明</summary>
        public string Description { get; set; }

        /// <summary>スコープ名</summary>
        public string ScopeName { get; set; }
    }
}