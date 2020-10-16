using System.Collections.Generic;
using System.Text.RegularExpressions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using RcrsCore.Api.IdentityServer.Admin.Biz;
using RcrsCore.Api.IdentityServer.Admin.Dto;
using RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel.Client;

namespace RcrsCore.Api.IdentityServer.Admin.Controllers
{
    //---------------------------------------------------------------
    /// <summary>
    /// クライアント管理コントローラー
    /// </summary>
    //---------------------------------------------------------------
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ClientController : Controller
    {
        /// <summary></summary>
        private readonly BizClient _bizClient;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="configurationDbContext"></param>
        //---------------------------------------------------------------
        public ClientController(ConfigurationDbContext configurationDbContext)
        {
            _bizClient = new BizClient(configurationDbContext);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアント全件取得
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("GetAll")]
        public MessageModel<List<ClientViewModel>> GetAll()
        {
            List<ClientViewModel> clientList = _bizClient.GetClient();

            return new MessageModel<List<ClientViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Response = clientList
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// IDでクライアントを取得します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("FindById")]
        public MessageModel<ClientViewModel> GetById(int id)
        {
            ClientViewModel client = _bizClient.FindByID(id);

            return new MessageModel<ClientViewModel>()
            {
                Msg = "OK",
                Success = true,
                Response = client
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 名称でクライアントを取得します。(全等)
        /// </summary>
        /// <param name="name">クライアント名</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("FindByName")]
        public MessageModel<ClientViewModel> GetByName(string name)
        {
            ClientViewModel client = _bizClient.FindByName(name);
            return new MessageModel<ClientViewModel>()
            {
                Msg = "OK",
                Success = true,
                Response = client
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 検索行います
        /// </summary>
        /// <param name="condition">検索条件</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("Search")]
        public MessageModel<List<ClientViewModel>> Search([FromBody] ClientSearchModel condition)
        {
            List<ClientViewModel> clientList = _bizClient.GetClient(condition);

            return new MessageModel<List<ClientViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Response = clientList
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// MVC型のクライアントを作成します。（付与タイプ：authorization code）
        /// </summary>
        /// <param name="viewModel">クライアント</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("CreateMVC")]
        public MessageModel<ClientViewModel> CreateMvc([FromBody] ClientViewModel viewModel)
        {
            return create(viewModel, IdentityConst.ClientType.MVC);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// JavaScript型のクライアントを作成します。（付与タイプ：authorization code）
        /// </summary>
        /// <param name="viewModel">クライアント</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("CreateJavaScript")]
        public MessageModel<ClientViewModel> CreateJavaScript([FromBody] ClientViewModel viewModel)
        {
            return create(viewModel, IdentityConst.ClientType.JavaScript);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 保存行います。
        /// (クライアント種類は固定ので、変更しても保存しません)
        /// </summary>
        /// <param name="viewClient">クライアント</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPut]
        [Route("Edit")]
        public MessageModel<ClientViewModel> Edit([FromBody] ClientViewModel viewClient)
        {
            MessageModel<ClientViewModel> messageModel = new MessageModel<ClientViewModel>();
            ClientViewModel client = _bizClient.FindByID(viewClient.ID);

            messageModel.Msg = string.Empty;
            messageModel.Response = viewClient;

            if (client != null)
            {
                //名称変更の場合
                if (client.Name != viewClient.Name)
                {
                    //名称合法判定
                    if (Regex.IsMatch(client.Name, "^[a-zA-Z0-9.]*$"))
                    {
                        //名称既存チェック
                        var sameNameClient = _bizClient.FindByName(viewClient.Name);
                        if (sameNameClient != null)
                        {
                            messageModel.Msg = "クライアント名既存しました。";
                            messageModel.Success = false;
                        }
                    }
                    else
                    {
                        messageModel.Msg = "クライアント名に英字と数字また「.」を入力してください。";
                        messageModel.Success = false;
                    }
                }

                if (string.IsNullOrEmpty(messageModel.Msg))
                {
                    messageModel.Response = _bizClient.SaveEdit(viewClient);
                    messageModel.Success = true;
                    messageModel.Msg = "OK";
                }
            }
            else
            {
                messageModel.Msg = "保存失敗（クライアントが見つかりませんでした）";
                messageModel.Success = false;
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 削除行います。
        /// </summary>
        /// <param name="id">クライアントId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("Delete")]
        public MessageModel<bool> Delete(int id)
        {
            var returnModel = new MessageModel<bool>();

            returnModel.Response = _bizClient.RemoveClient(id);

            if (returnModel.Response)
            {
                returnModel.Success = true;
                returnModel.Msg = "OK";
            }
            else
            {
                returnModel.Success = false;
                returnModel.Msg = "削除失敗";
            }

            return returnModel;
        }

        //=============================================================================> scope
        //---------------------------------------------------------------
        /// <summary>
        /// すべてスコープを取得します
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("Scope/GetAll")]
        public MessageModel<List<string>> GetClientScopeAll()
        {
            return new MessageModel<List<string>>()
            {
                Msg = "OK",
                Success = true,
                Response = _bizClient.GetAllScopes()
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアントのスコープリストを取得します。
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("Scope/GetListByClientId")]
        public MessageModel<List<string>> GetClientScopeList(int clientId)
        {
            return new MessageModel<List<string>>()
            {
                Msg = "OK",
                Success = true,
                Response = _bizClient.GetClientScopes(clientId)
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアント追加可能のスコープ一覧
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("Scope/GetClientAddable")]
        public MessageModel<List<string>> GetScopeListExceptCurrentClientUsed(int clientId)
        {
            List<string> scopeList = _bizClient.GetScopesForAdd(clientId);

            return new MessageModel<List<string>>()
            {
                Msg = "OK",
                Success = true,
                Response = scopeList
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// スコープをクライアントに追加します。
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="scope">スコープ</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("Scope/AddToClient")]
        public MessageModel<List<string>> AddScopeClient(int clientId, string scope)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            var client = _bizClient.FindByID(clientId);

            if (client == null)
            {
                messageModel.Msg = "クライアント存在していません。";
                messageModel.Success = false;
                messageModel.Response = new List<string> { scope };
            }
            else
            {
                if (client.ListScope.Contains(scope))
                {
                    messageModel.Msg = "スコープ既存しました。";
                    messageModel.Success = false;
                    messageModel.Response = client.ListScope;
                }
                else
                {
                    _bizClient.AddScopeToClient(clientId, scope);

                    messageModel.Msg = "OK";
                    messageModel.Success = true;
                    messageModel.Response = _bizClient.GetClientScopes(clientId);
                }
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// スコープをクライアントから削除します。
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="scope">スコープ</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("Scope/RemoveFromClient")]
        public MessageModel<List<string>> RemoveScopeFromClient(int clientId, string scope)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            var client = _bizClient.FindByID(clientId);

            if (client == null)
            {
                messageModel.Msg = "クライアント存在していません。";
                messageModel.Success = false;
                messageModel.Response = new List<string> { scope };
            }
            else
            {
                if (client.ListScope.Contains(scope))
                {
                    _bizClient.RemoveClientScopeByName(scope);

                    messageModel.Msg = "OK";
                    messageModel.Success = true;
                    messageModel.Response = _bizClient.GetClientScopes(clientId);
                }
                else
                {
                    messageModel.Msg = "スコープ存在しません。";
                    messageModel.Success = false;
                    messageModel.Response = client.ListScope;
                }
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアントを作成します
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        private MessageModel<ClientViewModel> create(ClientViewModel viewModel, string type)
        {
            MessageModel<ClientViewModel> resultMsg = new MessageModel<ClientViewModel>();
            resultMsg.Response = viewModel;

            if (Regex.IsMatch(viewModel.Name, "^[a-zA-Z0-9.]*$"))
            {
                var sameName = _bizClient.FindByName(viewModel.Name);
                if (sameName == null)
                {
                    Client client = convertViewToClient(viewModel, type);
                    bool isCreateSuccess = _bizClient.CreateClient(client.ToEntity()) != -1;

                    if (isCreateSuccess)
                    {
                        resultMsg.Msg = "新規成功";
                        resultMsg.Success = true;
                        resultMsg.Response = _bizClient.FindByName(viewModel.Name);
                    }
                    else
                    {
                        resultMsg.Msg = "新規失敗";
                        resultMsg.Success = false;
                    }
                }
                else
                {
                    resultMsg.Msg = "クライアント既存しました。";
                    resultMsg.Success = false;
                }
            }
            else
            {
                resultMsg.Msg = "クライアント名に英字と数字また「.」を入力してください。";
                resultMsg.Success = false;
            }

            return resultMsg;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ClientViewModel→Client
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        private Client convertViewToClient(ClientViewModel viewModel, string type)
        {
            Client client = new Client
            {
                ClientSecrets = { new Secret(IdentityConst.DefaultSecret.Sha256()) },

                ClientId = viewModel.Name,
                ClientUri = viewModel.ClientUri,
                Description = viewModel.Description,
                RequireConsent = false, //true時、同意画面を表示します。
                RequirePkce = true, //認証コードベースのトークンに証明キーが必要かどうかを指定します。
                RequireClientSecret = false, //falseに設定した場合、トークンエンドポイントでトークンを要求するためのクライアントシークレットは必要ありません。
                RedirectUris = { viewModel.RedirectUri },  //トークンまたは認証コードを返す許可されたURIを指定します。
                PostLogoutRedirectUris = { viewModel.PostLogoutRedirectUri }, //ログアウト後にリダイレクトする許可されたURIを指定します。
                AllowedScopes = IdentityConst.Scopes.ClientDefaultScopes, //クライアントがリクエストできるAPIスコープを指定します。 空の場合、クライアントはどのスコープにもアクセスできません。
            };

            if (type.Equals(IdentityConst.ClientType.MVC))
            {
                client.ClientName = IdentityConst.ClientType.MVC;
                client.AllowedGrantTypes = GrantTypes.Code;
                client.AlwaysIncludeUserClaimsInIdToken = true;
            }
            else if (type.Equals(IdentityConst.ClientType.JavaScript))
            {
                client.ClientName = IdentityConst.ClientType.JavaScript;
                client.AllowedGrantTypes = GrantTypes.Code;
            }

            return client;
        }
    }
}