using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace RcrsCore.IdentityServer.Dto.DomainModel.Application
{
    //---------------------------------------------------------------
    /// <summary>
    /// ロール(IdentityRoleから追記実装)
    /// </summary>
    //---------------------------------------------------------------
    public class ApplicationRole : IdentityRole<Guid>
    {
        /// <summary>分類</summary>
        public string TagCD { get; set; }

        /// <summary></summary>
        public string Description { get; set; }

        /// <summary></summary>
        public int OrderSort { get; set; }

        /// <summary></summary>
        public bool Enabled { get; set; } = true;

        /// <summary></summary>
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}