using System;
using System.Collections.Generic;
using System.Text;

namespace RcrsCore.Api.IdentityServer.Admin.Dto
{
    //-------------------------------------------------------------------------------
    /// <summary>
    /// 戻る用のメッセージモデル
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //-------------------------------------------------------------------------------
    public class MessageModel<T>
    {
        /// <summary>レスポンスステータスコード</summary>
        public int Status { get; set; } = 200;

        /// <summary>成功かどうか</summary>
        public bool Success { get; set; } = false;

        /// <summary>メッセージ</summary>
        public string Msg { get; set; } = "サーバーエラー";

        /// <summary>戻るのデータ</summary>
        public T Response { get; set; }
    }
}