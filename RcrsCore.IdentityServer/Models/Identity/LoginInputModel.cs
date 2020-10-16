using System.ComponentModel.DataAnnotations;

namespace RcrsCore.IdentityServer.Models.Identity
{
    //---------------------------------------------------------------
    /// <summary>
    /// ログイン時の入力関数
    /// </summary>
    //---------------------------------------------------------------
    public class LoginInputModel
    {
        /// <summary></summary>
        [Required]
        public string UserName { get; set; }

        /// <summary></summary>
        [Required]
        public string Password { get; set; }

        /// <summary></summary>
        public bool RememberLogin { get; set; }

        /// <summary></summary>
        public string ReturnUrl { get; set; }
    }
}