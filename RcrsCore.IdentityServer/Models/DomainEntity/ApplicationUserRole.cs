using System;
using Microsoft.AspNetCore.Identity;

namespace RcrsCore.IdentityServer.Models.DomainEntity
{
    //---------------------------------------------------------------
    /// <summary>
    /// ユーザーロール
    /// </summary>
    //---------------------------------------------------------------
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        /// <summary></summary>
        public virtual ApplicationUser User { get; set; }

        /// <summary></summary>
        public virtual ApplicationRole Role { get; set; }
    }
}