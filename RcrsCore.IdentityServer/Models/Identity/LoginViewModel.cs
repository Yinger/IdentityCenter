using System;
using System.Collections.Generic;
using System.Linq;

namespace RcrsCore.IdentityServer.Models.Identity
{
    //---------------------------------------------------------------
    /// <summary>
    /// 登録表示のモデルです。
    /// </summary>
    //---------------------------------------------------------------
    public class LoginViewModel : LoginInputModel
    {
        /// <summary>アカウントを記憶する</summary>
        public bool AllowRememberLogin { get; set; } = true;

        /// <summary></summary>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary></summary>
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();

        /// <summary></summary>
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

        /// <summary></summary>
        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;

        /// <summary></summary>
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }
}