using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using RcrsCore.Api.IdentityServer.Admin.Dto;
using RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel.Client;

namespace RcrsCore.Api.IdentityServer.Admin.Biz
{
    //---------------------------------------------------------------
    /// <summary>
    /// クライアントビズネスクラス
    /// </summary>
    //---------------------------------------------------------------
    public class BizClient
    {
        /// <summary></summary>
        private readonly ConfigurationDbContext _configContext;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="configurationDbContext"></param>
        //---------------------------------------------------------------
        public BizClient(ConfigurationDbContext configurationDbContext)
        {
            _configContext = configurationDbContext;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアント一覧を取得します。
        /// </summary>
        /// <param name="searchModel">検索条件</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<ClientViewModel> GetClient(ClientSearchModel searchModel = null)
        {
            List<Client> listClient = new List<Client>();
            List<ClientViewModel> clientViewModels = new List<ClientViewModel>();
            IQueryable<Client> query = (from client in _configContext.Clients.Include(i => i.AllowedGrantTypes) //付与タイプ
                                                                             .Include(i => i.RedirectUris) //LoginURI
                                                                             .Include(i => i.PostLogoutRedirectUris) //LogoutURI
                                                                             .Include(i => i.AllowedScopes) //スコープ
                                        select client).AsQueryable();

            if (searchModel != null)
            {
                //クライアント名
                if (!string.IsNullOrEmpty(searchModel.ClientName))
                    query = query.Where(x => x.ClientId.Contains(searchModel.ClientName));

                //クライアント種類（定数：MVC or JavaScript）
                if (!string.IsNullOrEmpty(searchModel.Type))
                    query = query.Where(x => x.ClientName.Equals(searchModel.Type));

                //説明
                if (!string.IsNullOrEmpty(searchModel.Description))
                    query = query.Where(x => x.Description.Contains(searchModel.Description));

                //URI
                if (!string.IsNullOrEmpty(searchModel.ClientUri))
                    query = query.Where(x => x.ClientUri.Contains(searchModel.ClientUri));
            }

            if (query.Any())
            {
                listClient = query.ToList();

                clientViewModels = (from client in listClient
                                    select new ClientViewModel
                                    {
                                        ID = client.Id,
                                        Name = client.ClientId,
                                        Type = client.ClientName,
                                        Description = client.Description,
                                        ClientUri = client.ClientUri,
                                        RedirectUri = client.RedirectUris.First().RedirectUri,
                                        PostLogoutRedirectUri = client.PostLogoutRedirectUris.First().PostLogoutRedirectUri,
                                        ListScope = client.AllowedScopes.Select(x => x.Scope).ToList()
                                    }).ToList();
            }

            return clientViewModels;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 名称でクライアントを取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ClientViewModel FindByName(string name)
        {
            ClientViewModel clientViewModel = null;

            name = name.ToLower();

            Client client = (from c in _configContext.Clients.Include(i => i.AllowedGrantTypes)
                                                             .Include(i => i.AllowedScopes)
                                                             .Include(i => i.RedirectUris)
                                                             .Include(i => i.PostLogoutRedirectUris)
                             where c.ClientId.ToLower().Equals(name)
                             select c).FirstOrDefault();

            if (client != null)
            {
                clientViewModel = new ClientViewModel();
                clientViewModel.ID = client.Id;
                clientViewModel.Name = client.ClientId;
                clientViewModel.Type = client.ClientName;
                clientViewModel.Description = client.Description;
                clientViewModel.ClientUri = client.ClientUri;
                clientViewModel.RedirectUri = client.RedirectUris.First().RedirectUri;
                clientViewModel.PostLogoutRedirectUri = client.PostLogoutRedirectUris.First().PostLogoutRedirectUri;
                clientViewModel.ListScope = client.AllowedScopes.Select(x => x.Scope).ToList();
            }

            return clientViewModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// IDでクライアントを取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ClientViewModel FindByID(int id)
        {
            ClientViewModel clientViewModel = null;
            Client client = findClientByID(id);

            if (client != null)
            {
                clientViewModel = new ClientViewModel();
                clientViewModel.ID = client.Id;
                clientViewModel.Name = client.ClientId;
                clientViewModel.Type = client.ClientName;
                clientViewModel.Description = client.Description;
                clientViewModel.ClientUri = client.ClientUri;
                clientViewModel.RedirectUri = client.RedirectUris.First().RedirectUri;
                clientViewModel.PostLogoutRedirectUri = client.PostLogoutRedirectUris.First().PostLogoutRedirectUri;
                clientViewModel.ListScope = client.AllowedScopes.Select(x => x.Scope).ToList();
            }

            return clientViewModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアントを新規します。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public int CreateClient(Client client)
        {
            _configContext.Clients.AddAsync(client);

            if (_configContext.SaveChanges() > 0)
                return client.Id;
            else
                return -1;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 編集を保存します。
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ClientViewModel SaveEdit(ClientViewModel viewModel)
        {
            Client client = findClientByID(viewModel.ID);

            if (client != null)
            {
                client.ClientId = viewModel.Name;
                client.Description = viewModel.Description;
                client.ClientUri = viewModel.ClientUri;
                client.RedirectUris.First().RedirectUri = viewModel.RedirectUri;
                client.PostLogoutRedirectUris.First().PostLogoutRedirectUri = viewModel.PostLogoutRedirectUri;

                _configContext.Clients.Update(client);
                _configContext.SaveChanges();
            }

            return FindByID(viewModel.ID);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアントを削除します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RemoveClient(int id)
        {
            Client client = findClientByID(id);

            if (client == null)
                return false;
            else
            {
                _configContext.Clients.Remove(client);
                _configContext.SaveChanges();

                return true;
            }
        }

        //---------------------------------------------------------------
        /// <summary>
        /// すべてのスコープを取得します。
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<string> GetAllScopes()
        {
            List<string> allScopes = IdentityConst.Scopes.ClientDefaultScopes;
            allScopes.AddRange(GetApiScopes());

            return allScopes;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアントのスコープを取得します。
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<string> GetClientScopes(int clientId)
        {
            Client client = findClientByID(clientId);
            List<string> listScopes = null;

            if (client.AllowedScopes.Any())
                listScopes = client.AllowedScopes.Select(x => x.Scope).ToList();

            return listScopes;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアント追加可能のスコープを取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<string> GetScopesForAdd(int id)
        {
            List<string> addedScopes = new List<string>(); //既に追加したのスコープ → ①
            List<string> allScopes = new List<string>(); //すべてのスコープ → ②
            List<string> exceptScopes = new List<string>(); //追加可能のスコープ → (② not in ①)

            //クライアントを取得
            Client client = findClientByID(id);

            //既に追加したのスコープを取得→ ①
            if (client.AllowedScopes != null)
                addedScopes = client.AllowedScopes.Select(x => x.Scope).ToList();

            //すべてのスコープを取得→ ②
            allScopes = IdentityConst.Scopes.ClientDefaultScopes;
            allScopes.AddRange(GetApiScopes());

            //追加可能のスコープを取得→ (② not in ①)
            exceptScopes = allScopes.Except(addedScopes).ToList();

            return exceptScopes;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クライアントにスコープを追加します。
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool AddScopeToClient(int clientId, string scopeName)
        {
            Client client = findClientByID(clientId);

            if (client == null)
                return false;

            client.AllowedScopes.Add(new ClientScope { Client = client, ClientId = client.Id, Scope = scopeName });
            _configContext.SaveChanges();

            return true;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// APIのスコープを削除時にクライアントのスコープも削除処理
        /// </summary>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RemoveClientScopeByName(string scopeName)
        {
            List<Client> listClient = (from c in _configContext.Clients.Include(i => i.AllowedGrantTypes)
                                                                       .Include(i => i.AllowedScopes)
                                                                       .Include(i => i.AllowedCorsOrigins)
                                                                       .Include(i => i.RedirectUris)
                                                                       .Include(i => i.PostLogoutRedirectUris)
                                       where c.AllowedScopes.Any() && c.AllowedScopes.Where(x => x.Scope.Equals(scopeName)).Count() > 0
                                       select c).ToList();

            if (listClient.Count > 0)
            {
                foreach (Client client in listClient)
                {
                    ClientScope scope = client.AllowedScopes.Where(x => x.Scope.Equals(scopeName)).First();
                    client.AllowedScopes.Remove(scope);
                }

                _configContext.SaveChanges();
            }

            return true;
        }

        ////---------------------------------------------------------------
        ///// <summary>
        ///// APIのスコープ名変更時にクライアントのスコープも変更処理
        ///// </summary>
        ///// <param name="oldName"></param>
        ///// <param name="newName"></param>
        ///// <returns></returns>
        ////---------------------------------------------------------------
        //public bool UpdateClientScopeByName(string oldName, string newName)
        //{
        //    List<Client> listClient = (from c in _configContext.Clients.Include(i => i.AllowedGrantTypes)
        //                                                               .Include(i => i.AllowedScopes)
        //                                                               .Include(i => i.AllowedCorsOrigins)
        //                                                               .Include(i => i.RedirectUris)
        //                                                               .Include(i => i.PostLogoutRedirectUris)
        //                               where c.AllowedScopes.Any() && c.AllowedScopes.Where(x => x.Scope.Equals(oldName)).Count() > 0
        //                               select c).ToList();

        //    if (listClient.Count > 0)
        //    {
        //        foreach (Client client in listClient)
        //        {
        //            ClientScope scope = client.AllowedScopes.Where(x => x.Scope.Equals(oldName)).First();
        //            scope.Scope = newName;
        //            _configContext.Update(scope);
        //        }

        //        _configContext.SaveChanges();
        //    }

        //    return true;
        //}

        //---------------------------------------------------------------
        /// <summary>
        /// IDでクライアントを取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        private Client findClientByID(int id)
        {
            Client client = (from c in _configContext.Clients.Include(i => i.AllowedGrantTypes)
                                                             .Include(i => i.AllowedScopes)
                                                             .Include(i => i.AllowedCorsOrigins)
                                                             .Include(i => i.RedirectUris)
                                                             .Include(i => i.PostLogoutRedirectUris)
                             where c.Id == id
                             select c).FirstOrDefault();

            return client;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// すべてAPIのすべてスコープを取得します。
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        private List<string> GetApiScopes()
        {
            BizApi bizApi = new BizApi(_configContext);
            List<string> apiScopes = bizApi.GetApiScopes();

            return apiScopes;
        }
    }
}