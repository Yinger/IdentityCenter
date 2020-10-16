using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RcrsCore.IdentityServer.Dto.ViewModel.Client
{
    //---------------------------------------------------------------
    /// <summary>
    /// クライアントモデル
    /// </summary>
    //---------------------------------------------------------------
    public class ClientViewModel
    {
        /// <summary>ID</summary>
        public int ID { get; set; }

        /// <summary>英字と数字また「.」入力可能</summary>
        [Required]
        [Display(Name = "クライアント名")]
        public string Name { get; set; }

        /// <summary>クライアント種類（定数：MVC or JavaScript）</summary>
        public string Type { get; set; }

        /// <summary>説明</summary>
        [Display(Name = "説明")]
        public string Description { get; set; }

        /// <summary>URI</summary>
        [Display(Name = "URI")]
        public string ClientUri { get; set; }

        /// <summary>where to redirect to after login</summary>
        [Display(Name = "RedirectUri")]
        public string RedirectUri { get; set; }

        /// <summary>where to redirect to after logout</summary>
        [Display(Name = "PostLogoutRedirectUri")]
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>スコープリスト</summary>
        public List<string> ListScope { get; set; }
    }
}