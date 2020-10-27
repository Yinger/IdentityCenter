using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RcrsCore.IdentityServer.Dto;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.IdentityServer.Dto.ViewModel.Role;
using RcrsCore.Api.IdentityServer.Admin.Biz;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application;

namespace RcrsCore.Api.IdentityServer.Admin.Controllers
{
    //---------------------------------------------------------------
    /// <summary>
    /// ユーザーとロール連動管理コントローラー
    /// </summary>
    //---------------------------------------------------------------
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserRoleController : Controller
    {
        /// <summary></summary>
        private readonly BizRole _bizRole;

        /// <summary></summary>
        private readonly BizUser _bizUser;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="applicationContext"></param>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
        //---------------------------------------------------------------
        public UserRoleController(ApplicationContext applicationContext, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _bizRole = new BizRole(applicationContext, roleManager);
            _bizUser = new BizUser(applicationContext, userManager, roleManager);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザー持っているのロール一覧を取得
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("GetUserRoleList")]
        public MessageModel<List<string>> GetUserRoleList(string userId)
        {
            List<RoleViewModel> roleList = _bizRole.GetRoleListByUserId(userId);

            return new MessageModel<List<string>>()
            {
                Msg = "OK",
                Success = true,
                Data = roleList.Select(x => x.RoleName).ToList()
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザー追加可能のロール一覧を取得
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("GetUserAddableRoleList")]
        public MessageModel<List<string>> GetRoleListExceptCurrentUserUsed(string userId)
        {
            List<RoleViewModel> roleList = _bizRole.GetRoleListExceptCurrentUserUsed(userId);

            return new MessageModel<List<string>>()
            {
                Msg = "OK",
                Success = true,
                Data = roleList.Select(x => x.RoleName).ToList()
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーにロールを追加します。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPut]
        [Route("AddRoleToUser")]
        public MessageModel<List<string>> AddRoleToUser(string userId, string roleName)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            messageModel.Data = new List<string>();

            if (string.IsNullOrEmpty(userId))
            {
                messageModel.Msg = "ユーザーIDはNULLです。";
                messageModel.Success = false;
            }
            else if (string.IsNullOrEmpty(roleName))
            {
                messageModel.Msg = "ロール名はNULLです。";
                messageModel.Success = false;
            }
            else
            {
                List<RoleViewModel> roleList = _bizRole.GetRoleListByUserId(userId);
                List<string> roleNameList = new List<string>();

                if (roleList != null)
                    roleNameList = roleList.Select(x => x.RoleName).ToList();

                if (roleNameList.Contains(roleName))
                {
                    messageModel.Msg = "ロール名既存しました。";
                    messageModel.Success = false;
                    messageModel.Data = roleNameList;
                }
                else
                {
                    roleList = _bizRole.GetRoleListExceptCurrentUserUsed(userId);
                    roleNameList = new List<string>();

                    if (roleList != null)
                        roleNameList = roleList.Select(x => x.RoleName).ToList();

                    if (!roleNameList.Contains(roleName))
                    {
                        messageModel.Msg = "システムに該当ロールがありません。";
                        messageModel.Success = false;
                        messageModel.Data = roleNameList;
                    }
                    else
                    {
                        //ユーザーにロールを追加します。
                        if (_bizUser.AddRole(userId, roleName))
                        {
                            messageModel.Msg = "OK";
                            messageModel.Success = true;
                            messageModel.Data = _bizRole.GetRoleListByUserId(userId).Select(x => x.RoleName).ToList();
                        }
                        else
                        {
                            messageModel.Msg = "追加失敗";
                            messageModel.Success = false;
                        }
                    }
                }
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーにロールを削除します。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("RemoveRoleFromUser")]
        public MessageModel<List<string>> RemoveRoleFromUser(string userId, string roleName)
        {
            MessageModel<List<string>> messageModel = new MessageModel<List<string>>();
            messageModel.Data = new List<string>();

            if (string.IsNullOrEmpty(userId))
            {
                messageModel.Msg = "ユーザーIDはNULLです。";
                messageModel.Success = false;
            }
            else if (string.IsNullOrEmpty(roleName))
            {
                messageModel.Msg = "ロール名はNULLです。";
                messageModel.Success = false;
            }
            else
            {
                List<RoleViewModel> roleList = _bizRole.GetRoleListByUserId(userId);
                List<string> roleNameList = new List<string>();

                if (roleList != null)
                    roleNameList = roleList.Select(x => x.RoleName).ToList();

                if (roleNameList.Contains(roleName))
                {
                    //削除行います。
                    if (_bizUser.RemoveRole(userId, roleName))
                    {
                        messageModel.Msg = "OK";
                        messageModel.Success = true;
                        messageModel.Data = _bizRole.GetRoleListByUserId(userId).Select(x => x.RoleName).ToList();
                    }
                    else
                    {
                        messageModel.Msg = "削除失敗";
                        messageModel.Success = false;
                    }
                }
                else
                {
                    messageModel.Msg = "ロールはユーザーに持っていません。";
                    messageModel.Success = false;
                    messageModel.Data = roleNameList;
                }
            }

            return messageModel;
        }

        ////---------------------------------------------------------------
        ///// <summary>
        ///// ユーザーのロールを一括更新します。
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="listUpdatedRoleName"></param>
        ///// <returns></returns>
        ////---------------------------------------------------------------
        //[HttpPost]
        //[Route("UpdateUserRole")]
        //public MessageModel<List<string>> UpdateUserRole(string userId, List<string> listUpdatedRoleName)
        //{
        //    MessageModel<List<string>> messageModel = new MessageModel<List<string>>();

        //    if (listUpdatedRoleName != null)
        //    {
        //        List<RoleViewModel> roleList = _bizRole.GetRoleListByUserId(userId);
        //        List<string> listOldRoleName = new List<string>();

        //        if (roleList != null)
        //            listOldRoleName = roleList.Select(x => x.RoleName).ToList();

        //        //listUpdatedRoleName有 listOldRoleName無 ⇒ 追加
        //        foreach (string roleName in listUpdatedRoleName)
        //        {
        //            if (!listOldRoleName.Contains(roleName))
        //                _bizUser.AddRole(userId, roleName);
        //        }

        //        //listUpdatedRoleName無 listOldRoleName有 ⇒ 削除
        //        foreach (string roleName in listOldRoleName)
        //        {
        //            if (!listUpdatedRoleName.Contains(roleName))
        //                _bizUser.RemoveRole(userId, roleName);
        //        }
        //    }

        //    messageModel.Msg = "OK";
        //    messageModel.Success = true;
        //    messageModel.Data = listUpdatedRoleName;

        //    return messageModel;
        //}
    }
}