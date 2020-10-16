using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel.Api
{
    //---------------------------------------------------------------
    /// <summary>
    /// Apiモデル
    /// </summary>
    //---------------------------------------------------------------
    public class ApiViewModel
    {
        /// <summary>Id</summary>
        public int Id { get; set; }

        /// <summary>英字と数字また「.」入力可能</summary>
        [Required]
        [Display(Name = "API名")]
        public string Name { get; set; }

        /// <summary>説明</summary>
        [Display(Name = "説明")]
        public string Description { get; set; }

        /// <summary>スコープリスト</summary>
        public List<string> ListScope { get; set; }

        /// <summary>クレーム</summary>
        public List<string> ListClaim { get; set; }
    }
}