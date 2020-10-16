using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application.Entity
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