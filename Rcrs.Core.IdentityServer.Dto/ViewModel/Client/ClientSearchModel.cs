namespace RcrsCore.IdentityServer.Dto.ViewModel.Client
{
    //---------------------------------------------------------------
    /// <summary>
    /// クライアント検索条件モデル
    /// </summary>
    //---------------------------------------------------------------
    public class ClientSearchModel
    {
        /// <summary>クライアント名</summary>
        public string ClientName { get; set; }

        /// <summary>クライアント種類（定数：MVC or JavaScript）</summary>
        public string Type { get; set; }

        /// <summary>説明</summary>
        public string Description { get; set; }

        /// <summary>URI</summary>
        public string ClientUri { get; set; }
    }
}