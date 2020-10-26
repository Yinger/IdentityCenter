using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.IdentityServer.Dto.ViewModel;
using RcrsCore.IdentityServer.Dto.ViewModel.Role;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application;

namespace RcrsCore.Api.IdentityServer.Admin.Biz
{
    //---------------------------------------------------------------
    /// <summary>
    /// ロールビズネスクラス
    /// </summary>
    //---------------------------------------------------------------
    public class BizRole
    {
        /// <summary></summary>
        private readonly ApplicationContext _roleContext;

        /// <summary></summary>
        private readonly RoleManager<ApplicationRole> _roleManager;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="roleContext"></param>
        /// <param name="roleManager"></param>
        //---------------------------------------------------------------
        public BizRole(ApplicationContext roleContext, RoleManager<ApplicationRole> roleManager)
        {
            _roleContext = roleContext;
            _roleManager = roleManager;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロール一覧を取得
        /// </summary>
        /// <param name="searchModel">検索条件</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<RoleViewModel> GetRoleList(RoleSearchModel searchModel = null)
        {
            IQueryable<RoleViewModel> roleQuery = (from role in _roleContext.Roles
                                                   select new RoleViewModel
                                                   {
                                                       Id = role.Id.ToString().ToUpper(),
                                                       RoleName = role.Name,
                                                       Tag = role.TagCD,
                                                       Description = role.Description,
                                                   }).AsQueryable();

            //検索行います。
            if (searchModel != null)
            {
                //ロール名
                if (!string.IsNullOrEmpty(searchModel.RoleName))
                    roleQuery = roleQuery.Where(x => x.RoleName.Contains(searchModel.RoleName));

                //説明
                if (!string.IsNullOrEmpty(searchModel.Description))
                    roleQuery = roleQuery.Where(x => x.Description.Contains(searchModel.Description));

                //タグ
                if (!string.IsNullOrEmpty(searchModel.Tag))
                    roleQuery = roleQuery.Where(x => x.Tag.Equals(searchModel.Tag));
            }

            return roleQuery.ToList();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロールリストを取得
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<SelectListItem> GetSelectList()
        {
            List<SelectListItem> roleList = (from role in _roleContext.Roles
                                             select new SelectListItem
                                             {
                                                 Value = role.Id.ToString(),
                                                 Text = role.Name
                                             }).ToList();

            //一行空値を入力します。
            roleList.Insert(0, new SelectListItem() { Value = "", Text = "---ロール---" });

            return roleList;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロールは既存するか判定します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public int GetRoleNameExistCount(string name)
        {
            return _roleContext.Roles.Where(x => x.Name.Equals(name)).Count();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロール名でロールを取得します。
        /// </summary>
        /// <param name="name">ロール名</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ApplicationRole FindByName(string name)
        {
            return _roleContext.Roles.Where(x => x.Name.Equals(name)).FirstOrDefault();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// IDでロールを取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public ApplicationRole FindByIdAsync(string id)
        {
            bool isValid = false;
            if (id != null)
            {
                id = id.ToUpper();
                Guid guidResult;
                isValid = Guid.TryParse(id, out guidResult);
            }

            if (isValid)
                return _roleManager.FindByIdAsync(id).Result;
            else
                return null;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロールを新規します。
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public IdentityResult CreateAsync(ApplicationRole role)
        {
            return _roleManager.CreateAsync(role).Result;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロールを更新します。
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public IdentityResult UpdateAsync(ApplicationRole role)
        {
            return _roleManager.UpdateAsync(role).Result;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロールを削除します。
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RemoveRole(ApplicationRole role)
        {
            bool result = false;

            if (role != null)
                result = _roleManager.DeleteAsync(role).Result.Succeeded;

            return result;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーのロール追加時にロール一覧を取得します。
        /// (既有のロール非表示します)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<RoleViewModel> GetRoleListExceptCurrentUserUsed(string userId, RoleSearchModel searchModel = null)
        {
            List<RoleViewModel> roleList = GetRoleList(searchModel);

            if (!string.IsNullOrEmpty(userId))
            {
                Guid guUserId = new Guid(userId);

                //ロール情報を取得します。
                List<ApplicationRole> userRoleList = (from role in _roleContext.Roles
                                                      from userRole in _roleContext.UserRoles.Where(x => x.UserId == guUserId).DefaultIfEmpty()
                                                      where role.Id == userRole.RoleId
                                                      select role).Distinct().ToList();
                //ユーザー既有のロール非表示処理
                roleList = (from role in roleList
                            from userRole in userRoleList.Where(x => x.Id.ToString().ToUpper() == role.Id.ToUpper()).DefaultIfEmpty()
                            where userRole == null
                            select role).ToList();
            }

            return roleList;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーのロールリストを取得します。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<RoleViewModel> GetRoleListByUserId(string userId)
        {
            Guid guid = new Guid(userId.ToUpper());
            List<RoleViewModel> listRole = (from role in _roleContext.Roles
                                            from userrole in _roleContext.UserRoles.Where(x => x.RoleId == role.Id).DefaultIfEmpty()
                                            where userrole != null && userrole.UserId == guid
                                            select new RoleViewModel
                                            {
                                                Id = role.Id.ToString(),
                                                RoleName = role.Name,
                                                Tag = role.TagCD,
                                                Description = role.Description
                                            }).ToList();

            return listRole;
        }
    }
}