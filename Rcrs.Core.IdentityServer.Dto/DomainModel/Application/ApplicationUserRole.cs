using Microsoft.AspNetCore.Identity;
using System;

namespace RcrsCore.IdentityServer.Dto.DomainModel.Application
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