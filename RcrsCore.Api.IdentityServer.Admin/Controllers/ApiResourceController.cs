using System.Collections.Generic;
using System.Text.RegularExpressions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using RcrsCore.IdentityServer.Dto;
using RcrsCore.IdentityServer.Dto.ViewModel.Api;
using RcrsCore.Api.IdentityServer.Admin.Biz;

namespace RcrsCore.Api.IdentityServer.Admin.Controllers
{
    //---------------------------------------------------------------
    /// <summary>
    /// API管理コントローラー
    /// </summary>
    //---------------------------------------------------------------
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ApiResourceController : Controller
    {
        /// <summary></summary>
        private readonly BizApi _bizApi;

        /// <summary></summary>
        private readonly BizClient _bizClient;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="configurationDbContext"></param>
        //---------------------------------------------------------------
        public ApiResourceController(ConfigurationDbContext configurationDbContext)
        {
            _bizApi = new BizApi(configurationDbContext);
            _bizClient = new BizClient(configurationDbContext);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// API全件取得
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("GetAll")]
        public MessageModel<List<ApiViewModel>> GetAll()
        {
            List<ApiViewModel> apiList = _bizApi.GetApi();

            return new MessageModel<List<ApiViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Data = apiList
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// IDでAPIを取得します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("FindById")]
        public MessageModel<ApiViewModel> GetById(int id)
        {
            ApiViewModel api = _bizApi.FindById(id);

            return new MessageModel<ApiViewModel>()
            {
                Msg = "OK",
                Success = true,
                Data = api
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 名称でAPIを取得します。(全等)
        /// </summary>
        /// <param name="name">API名</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("FindByName")]
        public MessageModel<ApiViewModel> GetByName(string name)
        {
            ApiViewModel api = _bizApi.FindByName(name);
            return new MessageModel<ApiViewModel>()
            {
                Msg = "OK",
                Success = true,
                Data = api
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
        public MessageModel<List<ApiViewModel>> Search([FromBody] ApiSearchModel condition)
        {
            List<ApiViewModel> apiList = _bizApi.GetApi(condition);

            return new MessageModel<List<ApiViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Data = apiList
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 新規行います。
        /// </summary>
        /// <param name="name">Api名（英字と数字また「.」入力可能）</param>
        /// <param name="desc">説明</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("Create")]
        public MessageModel<ApiViewModel> Create(string name, string desc)
        {
            ApiViewModel apiViewModel = new ApiViewModel();
            MessageModel<ApiViewModel> messageModel = new MessageModel<ApiViewModel>();

            apiViewModel.Name = name;
            apiViewModel.Description = desc;

            //名称合法判定
            if (Regex.IsMatch(name, "^[a-zA-Z0-9.]*$"))
            {
                //名称既存チェック
                var apiInDb = _bizApi.FindByName(name);
                if (apiInDb == null)
                {
                    //ApiResourceエンティティ作成
                    var api = new ApiResource
                    {
                        Name = name,
                        Description = desc ?? "",
                        Scopes = { new Scope { Name = name, UserClaims = IdentityConst.CustomJwtClaimTypes.DefaultClaimTypes } },
                        UserClaims = IdentityConst.CustomJwtClaimTypes.DefaultClaimTypes,
                        ApiSecrets = new List<Secret>() { new Secret(IdentityConst.DefaultSecret.Sha256()) }
                    };

                    //新規します。
                    apiViewModel = _bizApi.Create(api.ToEntity());

                    if (apiViewModel.Id == -1)
                    {
                        messageModel.Msg = "新規失敗";
                        messageModel.Success = false;
                    }
                    else
                    {
                        messageModel.Msg = "OK";
                        messageModel.Success = true;
                    }
                }
                else
                {
                    //名称既存の場合
                    messageModel.Msg = "API既存しました。";
                    messageModel.Success = false;
                }
            }
            else
            {
                //名称合法判定失敗の場合
                messageModel.Msg = "API名に英字と数字また「.」を入力してください。";
                messageModel.Success = false;
            }

            messageModel.Data = apiViewModel;

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 保存行います。
        /// (スコープとクレームは別途で管理ので、空のまま保存大丈夫です。)
        /// </summary>
        /// <param name="viewApi">API</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPut]
        [Route("Edit")]
        public MessageModel<ApiViewModel> Edit([FromBody] ApiViewModel viewApi)
        {
            MessageModel<ApiViewModel> messageModel = new MessageModel<ApiViewModel>();
            ApiViewModel api = _bizApi.FindById(viewApi.Id);

            messageModel.Msg = string.Empty;
            messageModel.Data = viewApi;

            if (api != null)
            {
                //名称変更の場合
                if (api.Name != viewApi.Name)
                {
                    //名称合法判定
                    if (Regex.IsMatch(viewApi.Name, "^[a-zA-Z0-9.]*$"))
                    {
                        //名称既存チェック
                        var sameNameApi = _bizApi.FindByName(viewApi.Name);
                        if (sameNameApi != null)
                        {
                            messageModel.Msg = "API既存しました。";
                            messageModel.Success = false;
                        }
                    }
                    else
                    {
                        messageModel.Msg = "API名に英字と数字また「.」を入力してください。";
                        messageModel.Success = false;
                    }
                }

                if (string.IsNullOrEmpty(messageModel.Msg))
                {
                    api.Name = viewApi.Name;
                    api.Description = viewApi.Description;
                    messageModel.Data = _bizApi.SaveEdit(api);
                    messageModel.Success = true;
                    messageModel.Msg = "OK";

                    //TODO:名称変更後、別のAPIのscope?
                }
            }
            else
            {
                messageModel.Msg = "保存失敗（APIが見つかりませんでした）";
                messageModel.Success = false;
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 削除行います。
        /// </summary>
        /// <param name="id">ApiId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("Delete")]
        public MessageModel<bool> Delete(int id)
        {
            var returnModel = new MessageModel<bool>();
            ApiViewModel viewModel = _bizApi.FindById(id);

            returnModel.Data = _bizApi.RemoveApi(id);

            if (returnModel.Data)
            {
                ////クライアントのスコープも削除処理
                //foreach (string scope in viewModel.ListScope)
                //    _bizClient.RemoveClientScopeByName(scope);

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
        /// すべてAPIのすべてスコープを取得します
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("Scope/GetAll")]
        public MessageModel<List<string>> GetApiScopeAll()
        {
            return new MessageModel<List<string>>()
            {
                Msg = "OK",
                Success = true,
                Data = _bizApi.GetApiScopes()
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Apiのスコープリストを取得します。
        /// </summary>
        /// <param name="apiId">ApiID</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("Scope/GetListByApiId")]
        public MessageModel<List<string>> GetApiScopeList(int apiId)
        {
            ApiViewModel api = _bizApi.FindById(apiId);
            List<string> listScope = new List<string>();

            if (api != null)
                listScope = api.ListScope;

            return new MessageModel<List<string>>()
            {
                Msg = "OK",
                Success = true,
                Data = listScope
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// スコープをApiResourceに追加します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <param name="scope">スコープ</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("Scope/AddToApi")]
        public MessageModel<List<string>> AddScopeToApi(int apiId, string scope)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            var api = _bizApi.FindById(apiId);

            if (api == null)
            {
                messageModel.Msg = "API存在していません。";
                messageModel.Success = false;
                messageModel.Data = new List<string> { scope };
            }
            else
            {
                if (_bizApi.AddScopeToApi(apiId, scope))
                    return GetApiScopeList(apiId);
                else
                {
                    messageModel.Msg = "スコープ既存しました。";
                    messageModel.Success = false;
                    messageModel.Data = api.ListScope;
                }
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// スコープをApiResourceから削除します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <param name="scope">スコープ</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("Scope/RemoveFromApi")]
        public MessageModel<List<string>> RemoveScopeFromApi(int apiId, string scope)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            var api = _bizApi.FindById(apiId);

            if (api == null)
            {
                messageModel.Msg = "API存在していません。";
                messageModel.Success = false;
                messageModel.Data = new List<string> { scope };
            }
            else
            {
                if (_bizApi.RemoveScopeFromApi(apiId, scope))
                {
                    //_bizClient.RemoveClientScopeByName(scope); //クライアントのスコープも削除処理

                    return GetApiScopeList(apiId);
                }
                else
                {
                    messageModel.Msg = "スコープ存在しません。";
                    messageModel.Success = false;
                    messageModel.Data = api.ListScope;
                }
            }

            return messageModel;
        }

        //=============================================================================> UserClaims
        //---------------------------------------------------------------
        /// <summary>
        /// クレーム一覧を取得します。
        /// (全項目に可用クレームは固定です。)
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("Claim/GetAll")]
        public MessageModel<List<string>> GetClaimsAll()
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();

            messageModel.Msg = "OK";
            messageModel.Success = true;
            messageModel.Data = IdentityConst.CustomJwtClaimTypes.DefaultClaimTypes;

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIに追加可能のクレーム一覧を取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("Claim/GetApiAddable")]
        public MessageModel<List<string>> GetApiAddableClaims(int apiId)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            var api = _bizApi.FindById(apiId);

            if (api == null)
            {
                messageModel.Msg = "API存在していません。";
                messageModel.Success = false;
                messageModel.Data = new List<string> { };
            }
            else
            {
                messageModel.Msg = "OK";
                messageModel.Success = true;
                messageModel.Data = _bizApi.GetApiClaimsForAdd(apiId);
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Apiのクレームリストを取得します。
        /// </summary>
        /// <param name="apiId">ApiID</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("Claim/GetListByApiId")]
        public MessageModel<List<string>> GetApiClaimList(int apiId)
        {
            ApiViewModel api = _bizApi.FindById(apiId);
            List<string> listClaim = new List<string>();

            if (api != null)
                listClaim = api.ListClaim;

            return new MessageModel<List<string>>()
            {
                Msg = "OK",
                Success = true,
                Data = listClaim
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// スコープをApiResourceに追加します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <param name="claim">クレーム</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("Claim/AddToApi")]
        public MessageModel<List<string>> AddClaimToApi(int apiId, string claim)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            var api = _bizApi.FindById(apiId);

            if (api == null)
            {
                messageModel.Msg = "API存在していません。";
                messageModel.Success = false;
                messageModel.Data = new List<string> { claim };
            }
            else
            {
                if (IdentityConst.CustomJwtClaimTypes.DefaultClaimTypes.Contains(claim))
                {
                    if (_bizApi.AddClaimToApi(apiId, claim))
                        return GetApiClaimList(apiId);
                    else
                    {
                        messageModel.Msg = "クレーム既存しました。";
                        messageModel.Success = false;
                        messageModel.Data = api.ListScope;
                    }
                }
                else
                {
                    messageModel.Msg = "このクレームはシステムに存在していません。";
                    messageModel.Success = false;
                    messageModel.Data = new List<string> { claim };
                }
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// スコープをApiResourceから削除します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <param name="claim">クレーム</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("Claim/RemoveFromApi")]
        public MessageModel<List<string>> RemoveClaimFromApi(int apiId, string claim)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            var api = _bizApi.FindById(apiId);

            if (api == null)
            {
                messageModel.Msg = "API存在していません。";
                messageModel.Success = false;
                messageModel.Data = new List<string> { claim };
            }
            else
            {
                if (_bizApi.RemoveApiClaim(apiId, claim))
                    return GetApiClaimList(apiId);
                else
                {
                    messageModel.Msg = "クレーム存在しません。";
                    messageModel.Success = false;
                    messageModel.Data = new List<string> { claim };
                }
            }

            return messageModel;
        }
    }
}