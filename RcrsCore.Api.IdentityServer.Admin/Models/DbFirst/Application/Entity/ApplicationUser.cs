using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application.Entity
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