using System;
using System.Collections.Generic;

namespace RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.City.Entity
{
    public partial class M_市区町村
    {
        public long Id { get; set; }
        public string 市区町村cd { get; set; }
        public string 都道府県cd { get; set; }
        public string 都道府県名 { get; set; }
        public string 市区町村名 { get; set; }
        public string 郵便番号 { get; set; }
        public string 住所 { get; set; }
        public string メモ { get; set; }
    }
}