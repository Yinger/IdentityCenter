using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RcrsCore.IdentityServer.Dto;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.IdentityServer.Dto.ViewModel;
using RcrsCore.IdentityServer.Dto.ViewModel.Role;
using RcrsCore.Api.IdentityServer.Admin.Biz;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application;

namespace RcrsCore.Api.IdentityServer.Admin.Controllers
{
    //---------------------------------------------------------------
    /// <summary>
    /// ロール管理コントローラー
    /// </summary>
    //---------------------------------------------------------------
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class RoleController : Controller
    {
        /// <summary></summary>
        private readonly BizRole _bizRole;

        /// <summary></summary>
        private readonly BizUserClaims _bizUserClaim;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="userRoleContext"></param>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
        //---------------------------------------------------------------
        public RoleController(ApplicationContext userRoleContext, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _bizRole = new BizRole(userRoleContext, roleManager);
            _bizUserClaim = new BizUserClaims(userManager, userRoleContext);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロール全件取得
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("GetAll")]
        public MessageModel<List<RoleViewModel>> GetAll()
        {
            List<RoleViewModel> roleList = _bizRole.GetRoleList();

            return new MessageModel<List<RoleViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Data = roleList
            };
        }

        ////---------------------------------------------------------------
        ///// <summary>
        ///// ユーザー追加可能のロール一覧
        ///// </summary>
        ///// <param name="userId">UserId</param>
        ///// <returns></returns>
        ////---------------------------------------------------------------
        //[HttpGet]
        //[Route("GetUserAddable")]
        //public MessageModel<List<RoleViewModel>> GetRoleListExceptCurrentUserUsed(string userId)
        //{
        //    List<RoleViewModel> roleList = _bizRole.GetRoleListExceptCurrentUserUsed(userId);

        //    return new MessageModel<List<RoleViewModel>>()
        //    {
        //        Msg = "OK",
        //        Success = true,
        //        Response = roleList
        //    };
        //}

        //---------------------------------------------------------------
        /// <summary>
        /// IDでロールを取得します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("FindById")]
        public MessageModel<RoleViewModel> FindById(string id)
        {
            RoleViewModel role = null;
            ApplicationRole appRole = _bizRole.FindByIdAsync(id);

            if (appRole != null)
            {
                role = new RoleViewModel();

                role.Id = appRole.Id.ToString();
                role.RoleName = appRole.Name;
                role.Tag = appRole.TagCD;
                role.Description = appRole.Description;
            }

            return new MessageModel<RoleViewModel>()
            {
                Msg = "OK",
                Success = true,
                Data = role
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 名称でロールを取得します。
        /// </summary>
        /// <param name="name">ロール名</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("FindByName")]
        public MessageModel<RoleViewModel> FindByName(string name)
        {
            RoleViewModel role = null;
            ApplicationRole appRole = _bizRole.FindByName(name);

            if (appRole != null)
            {
                role = new RoleViewModel();

                role.Id = appRole.Id.ToString();
                role.RoleName = appRole.Name;
                role.Tag = appRole.TagCD;
                role.Description = appRole.Description;
            }

            return new MessageModel<RoleViewModel>()
            {
                Msg = "OK",
                Success = true,
                Data = role
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
        public MessageModel<List<RoleViewModel>> Search([FromBody] RoleSearchModel condition)
        {
            List<RoleViewModel> roleList = _bizRole.GetRoleList(condition);

            return new MessageModel<List<RoleViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Data = roleList
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 新規行います。
        /// </summary>
        /// <param name="viewRole">ロール</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("Create")]
        public MessageModel<RoleViewModel> Create([FromBody] RoleViewModel viewRole)
        {
            var returnModel = new MessageModel<RoleViewModel>();
            ApplicationRole role = new ApplicationRole();

            //重名チェック
            if (_bizRole.GetRoleNameExistCount(viewRole.RoleName) > 0)
            {
                returnModel.Success = false;
                returnModel.Msg = "該当ロール名は既存しました。";
            }
            else
            {
                //新規作成
                role.Name = viewRole.RoleName;
                role.TagCD = viewRole.Tag;
                role.Description = viewRole.Description;

                //保存
                returnModel.Success = _bizRole.CreateAsync(role).Succeeded;
            }

            if (returnModel.Success)
            {
                viewRole.Id = role.Id.ToString();
                returnModel.Msg = "新規成功";
            }

            returnModel.Data = viewRole;

            return returnModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 保存行います。
        /// </summary>
        /// <param name="viewRole">ロール</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPut]
        [Route("Edit")]
        public MessageModel<RoleViewModel> SaveEdit([FromBody] RoleViewModel viewRole)
        {
            ApplicationRole appRole = null;
            var returnModel = new MessageModel<RoleViewModel>();
            string roleOldName = string.Empty;
            returnModel.Data = viewRole;

            //必須入力チェック
            if (viewRole != null && !string.IsNullOrEmpty(viewRole.RoleName))
            {
                //編集対象のDBモデルを取得
                appRole = _bizRole.FindByIdAsync(viewRole.Id);

                //既存の場合、編集保存行います。
                if (appRole != null)
                {
                    //名称変更の場合、重名チェック、AspNetUserClaimsのロールを更新
                    if (appRole.Name != viewRole.RoleName)
                    {
                        //ロール名既存判定
                        if (_bizRole.GetRoleNameExistCount(viewRole.RoleName) > 1)
                        {
                            returnModel.Success = false;
                            returnModel.Msg = "該当ロール名は既存しました。";
                        }
                    }

                    //保存行います。
                    if (string.IsNullOrEmpty(returnModel.Msg))
                    {
                        roleOldName = appRole.Name;

                        appRole.Name = viewRole.RoleName;
                        appRole.TagCD = viewRole.Tag;
                        appRole.Description = viewRole.Description;

                        returnModel.Success = _bizRole.UpdateAsync(appRole).Succeeded;
                    }

                    //保存成功の後処理
                    if (returnModel.Success)
                    {
                        //名称変更の場合、AspNetUserClaimsのロールを更新します。
                        if (!string.IsNullOrEmpty(roleOldName))
                            _bizUserClaim.UpdateRoleName(roleOldName, viewRole.RoleName);

                        returnModel.Msg = "保存成功";
                        returnModel.Data = viewRole;
                    }
                }
                else
                {
                    returnModel.Success = false;
                    returnModel.Msg = "保存失敗（ロールが見つかりませんでした）";
                }
            }
            else
            {
                returnModel.Success = false;
                returnModel.Msg = "保存失敗（「ロール名」を入力してください）";
            }

            return returnModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// IDで削除行います。
        /// </summary>
        /// <param name="id">ロールId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("DeleteById")]
        public MessageModel<bool> DeleteById(string id)
        {
            var returnModel = new MessageModel<bool>();
            ApplicationRole role = _bizRole.FindByIdAsync(id);

            //ロールを削除します。
            returnModel.Data = _bizRole.RemoveRole(role);
            //ユーザーのクレームを更新します。
            if (returnModel.Data)
            {
                _bizUserClaim.RemoveRoleName(role.Name);
                returnModel.Success = true;
                returnModel.Msg = "削除成功";
            }
            else
            {
                returnModel.Success = false;
                returnModel.Msg = "削除失敗";
            }

            return returnModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 名称で削除行います。
        /// </summary>
        /// <param name="name">ロール名</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("DeleteByName")]
        public MessageModel<bool> DeleteByName(string name)
        {
            var returnModel = new MessageModel<bool>();
            ApplicationRole role = _bizRole.FindByName(name);

            //ロールを削除します。
            returnModel.Data = _bizRole.RemoveRole(role);
            //ユーザーのクレームを更新します。
            if (returnModel.Data)
            {
                _bizUserClaim.RemoveRoleName(role.Name);
                returnModel.Success = true;
                returnModel.Msg = "削除成功";
            }
            else
            {
                returnModel.Success = false;
                returnModel.Msg = "削除失敗";
            }

            return returnModel;
        }
    }
}