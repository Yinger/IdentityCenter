using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RcrsCore.IdentityServer.Dto.ViewModel.User
{
    //---------------------------------------------------------------
    /// <summary>
    /// ユーザーモデル
    /// </summary>
    //---------------------------------------------------------------
    public class UserViewModel
    {
        /// <summary>Id</summary>
        public string Id { get; set; }

        /// <summary>ユーザー名</summary>
        [Required]
        public string LoginName { get; set; }

        /// <summary>メールアドレス</summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>市区町村</summary>
        public string LgCode { get; set; }

        /// <summary>所属課</summary>
        public string LgKaKakari { get; set; }

        /// <summary>ロール</summary>
        public List<string> ListRole { get; set; }

        /// <summary>クレーム</summary>
        public List<string> ListClaim { get; set; }
    }
}