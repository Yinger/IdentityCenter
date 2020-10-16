using System.ComponentModel.DataAnnotations;

namespace RcrsCore.IdentityServer.Dto.ViewModel.Role
{
    //---------------------------------------------------------------
    /// <summary>
    /// ロール
    /// </summary>
    //---------------------------------------------------------------
    public class RoleViewModel
    {
        /// <summary>Id</summary>
        [Required]
        public string Id { get; set; }

        /// <summary>ロール名</summary>
        [Required]
        public string RoleName { get; set; }

        /// <summary>タグ</summary>
        public string Tag { get; set; }

        /// <summary>説明</summary>
        public string Description { get; set; }
    }
}