using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using RcrsCore.Api.IdentityServer.Admin.Dto;
using RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel.Api;

namespace RcrsCore.Api.IdentityServer.Admin.Biz
{
    //---------------------------------------------------------------
    /// <summary>
    /// APIビズネスクラス
    /// </summary>
    //---------------------------------------------------------------
    public class BizApi
    {
        /// <summary></summary>
        private readonly ConfigurationDbContext _configContext;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="configurationDbContext"></param>
        //---------------------------------------------------------------
        public BizApi(ConfigurationDbContext configurationDbContext)
        {
            _configContext = configurationDbContext;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Api一覧を取得します。
        /// </summary>
        /// <param name="searchModel">検索条件</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<ApiViewModel> GetApi(ApiSearchModel searchModel = null)
        {
            List<ApiResource> listApiResource = new List<ApiResource>();
            List<ApiViewModel> listApiView = new List<ApiViewModel>();
            IQueryable<ApiResource> queryApi = (from api in _configContext.ApiResources.Include(s => s.Scopes).ThenInclude(sc => sc.UserClaims)
                                                                                       .Include(c => c.UserClaims)
                                                select api).AsQueryable();

            //検索行います。
            if (searchModel != null)
            {
                //API名
                if (!string.IsNullOrEmpty(searchModel.ApiName))
                    queryApi = queryApi.Where(x => x.Name.Contains(searchModel.ApiName));

                //説明
                if (!string.IsNullOrEmpty(searchModel.Description))
                    queryApi = queryApi.Where(x => x.Description.Contains(searchModel.Description));

                //スコープ名
                if (!string.IsNullOrEmpty(searchModel.ScopeName))
                    queryApi = queryApi.Where(x => (x.Scopes != null && x.Scopes.Where(x => x.Name.Contains(searchModel.ScopeName)).Count() > 0));
            }

            if (queryApi.Any())
            {
                listApiResource = queryApi.ToList();

                listApiView = (from api in listApiResource
                               select new ApiViewModel
                               {
                                   Id = api.Id,
                                   Name = api.Name,
                                   Description = api.Description,
                                   ListScope = api.Scopes.Select(x => x.Name).ToList(),
                                   ListClaim = api.UserClaims.Select(x => x.Type).ToList()
                               }).ToList();
            }

            return listApiView;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// IDでAPIを取得します。
        /// </summary>
        /// <param name="id">APIのID</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ApiViewModel FindById(int id)
        {
            ApiViewModel viewApi = null;
            ApiResource api = (from ar in _configContext.ApiResources.Include(i => i.Scopes).ThenInclude(t => t.UserClaims)
                                                                        .Include(i => i.UserClaims)
                               where ar.Id == id
                               select ar).FirstOrDefault();

            if (api != null)
            {
                viewApi = new ApiViewModel();

                viewApi.Id = api.Id;
                viewApi.Name = api.Name;
                viewApi.Description = api.Description;
                viewApi.ListScope = api.Scopes.Select(x => x.Name).ToList();
                viewApi.ListClaim = api.UserClaims.Select(x => x.Type).ToList();
            }

            return viewApi;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 名称でAPIを取得します。(全等)
        /// </summary>
        /// <param name="name">APIの名称</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ApiViewModel FindByName(string name)
        {
            ApiViewModel viewApi = null;
            ApiResource api = (from ar in _configContext.ApiResources.Include(i => i.Scopes).ThenInclude(t => t.UserClaims)
                                                                        .Include(i => i.UserClaims)
                               where ar.Name == name
                               select ar).FirstOrDefault();

            if (api != null)
            {
                viewApi = new ApiViewModel();
                viewApi.Id = api.Id;
                viewApi.Name = api.Name;
                viewApi.Description = api.Description;
                viewApi.ListScope = api.Scopes.Select(x => x.Name).ToList();
                viewApi.ListClaim = api.UserClaims.Select(x => x.Type).ToList();
            }

            return viewApi;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIを新規します。
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ApiViewModel Create(ApiResource api)
        {
            ApiViewModel viewModel = new ApiViewModel();

            _configContext.ApiResources.AddAsync(api);

            if (_configContext.SaveChanges() > 0)
            {
                viewModel.Id = api.Id;
                viewModel.Name = api.Name;
                viewModel.Description = api.Description;
                viewModel.ListScope = api.Scopes.Select(s => s.Name).ToList();
                viewModel.ListClaim = api.UserClaims.Select(u => u.Type).ToList();
            }
            else
            {
                viewModel.Id = -1;
                viewModel.Name = api.Name;
                viewModel.Description = api.Description;
            }

            return viewModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIを保存します。
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ApiViewModel SaveEdit(ApiViewModel viewModel)
        {
            ApiResource api = (from ar in _configContext.ApiResources.Include(i => i.Scopes).ThenInclude(t => t.UserClaims)
                                                                     .Include(i => i.UserClaims)
                               where ar.Id == viewModel.Id
                               select ar).FirstOrDefault();

            api.Name = viewModel.Name;
            api.Description = viewModel.Description;

            _configContext.ApiResources.Update(api);
            _configContext.SaveChanges();

            return FindById(viewModel.Id);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIを削除します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RemoveApi(int id)
        {
            ApiResource api = _configContext.ApiResources.Find(id);

            //既存の場合、削除行います。
            if (api != null)
            {
                _configContext.ApiResources.Remove(api);
                _configContext.SaveChanges();

                return true;
            }

            return false;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// すべてAPIのすべてスコープを取得します。
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<string> GetApiScopes()
        {
            List<ApiResource> apiRsList = new List<ApiResource>();
            List<string> apiScopes = new List<string>();

            apiRsList = (from api in _configContext.ApiResources.Include(i => i.Scopes)
                         where api.Scopes != null
                         select api).ToList();

            foreach (ApiResource api in apiRsList)
            {
                foreach (ApiScope scope in api.Scopes)
                    apiScopes.Add(scope.Name);
            }

            apiScopes.Distinct();

            return apiScopes;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// スコープを追加します。
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool AddScopeToApi(int apiId, string scopeName)
        {
            ApiResource api = FindApiById(apiId);

            if (api == null)
                return false;
            else
            {
                //API内同名チェック
                if (api.Scopes.Where(x => x.Name.Equals(scopeName)).Count() > 0)
                    return false;

                //スコープを作成し、APIへ追加します。
                ApiScope scope = new ApiScope();
                scope.ApiResource = api;
                scope.ApiResourceId = apiId;
                scope.Name = scopeName;

                api.Scopes.Add(scope);

                _configContext.SaveChanges();

                return true;
            }
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIのスコープを削除します。
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RemoveScopeFromApi(int apiId, string scopeName)
        {
            ApiResource api = FindApiById(apiId);

            if (api == null)
                return false;
            else
            {
                var apiScope = api.Scopes.Where(x => x.Name.Equals(scopeName)).FirstOrDefault();

                if (apiScope == null) //既存チェック
                    return false;

                api.Scopes.Remove(apiScope);
                _configContext.SaveChanges();

                return true;
            }
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIの既存スコープに指定スコープ名のスコープを取得します。
        /// </summary>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ApiScope GetApiScopeByName(string scopeName)
        {
            ApiResource apiResource = (from api in _configContext.ApiResources.Include(i => i.Scopes)
                                       where api.Scopes.Any() && api.Scopes.Where(x => x.Name.Equals(scopeName)).Count() > 0
                                       select api).FirstOrDefault();

            if (apiResource == null)
                return null;
            else
                return apiResource.Scopes.Where(x => x.Name.Equals(scopeName)).FirstOrDefault();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIに追加可能のクレームを取得します。
        /// </summary>
        /// <param name="apiId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<string> GetApiClaimsForAdd(int apiId)
        {
            List<string> addedClaims = new List<string>();
            List<string> allClaims = IdentityConst.CustomJwtClaimTypes.DefaultClaimTypes;
            List<string> exceptClaims = new List<string>();

            ApiResource api = FindApiById(apiId);
            if (api != null && api.UserClaims != null)
                addedClaims = api.UserClaims.Select(x => x.Type).ToList();

            exceptClaims = allClaims.Except(addedClaims).ToList();

            return exceptClaims;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クレームを追加します。
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool AddClaimToApi(int apiId, string claimType)
        {
            ApiResource api = FindApiById(apiId);

            if (api == null)
                return false;
            else
            {
                if (api.UserClaims.Where(x => x.Type.Equals(claimType)).Count() > 0)
                    return false;

                ApiResourceClaim claim = new ApiResourceClaim();
                claim.ApiResource = api;
                claim.ApiResourceId = apiId;
                claim.Type = claimType;

                api.UserClaims.Add(claim);

                _configContext.SaveChanges();

                return true;
            }
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIのクレームを削除します。
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RemoveApiClaim(int apiId, string claim)
        {
            ApiResource api = FindApiById(apiId);

            if (api != null)
            {
                if (api.UserClaims.Any())
                {
                    ApiResourceClaim apiClaim = api.UserClaims.Where(x => x.Type.Equals(claim)).FirstOrDefault();

                    if (apiClaim != null)
                    {
                        api.UserClaims.Remove(apiClaim);
                        _configContext.SaveChanges();
                        return true;
                    }
                }
            }

            return false;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クレームIDでAPIのクレームを取得します。
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="claimId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ApiResourceClaim GetApiClaimById(int apiId, int claimId)
        {
            ApiResource api = FindApiById(apiId);
            ApiResourceClaim claim = null;

            if (api != null && api.UserClaims.Any())
                claim = api.UserClaims.Where(x => x.Id.Equals(claimId)).FirstOrDefault();

            return claim;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// IdでAPIを取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        private ApiResource FindApiById(int id)
        {
            //スコープ情報(スコープのユーザークレーム含む)、ユーザークレーム情報含むAPIリソースを取得します。
            ApiResource apiRs = (from api in _configContext.ApiResources.Include(i => i.Scopes).ThenInclude(t => t.UserClaims)
                                                                        .Include(i => i.UserClaims)
                                 where api.Id == id
                                 select api).FirstOrDefault();
            return apiRs;
        }
    }
}