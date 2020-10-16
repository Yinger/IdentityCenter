using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace RcrsCore.IdentityServer.Models.DomainEntity
{
    //---------------------------------------------------------------
    /// <summary>
    /// ユーザーモデル(IdentityUserから追記実装)
    /// </summary>
    //---------------------------------------------------------------
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary></summary>
        public string LgCode { get; set; }

        /// <summary></summary>
        public string LgKaKakari { get; set; }

        /// <summary></summary>
        public string FirstName { get; set; }

        /// <summary></summary>
        public string LastName { get; set; }

        /// <summary></summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary></summary>
        public bool IsChiiki { get; set; } = false;

        /// <summary></summary>
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}