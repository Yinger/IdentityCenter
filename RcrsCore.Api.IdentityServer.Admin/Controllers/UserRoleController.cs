﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RcrsCore.Api.IdentityServer.Admin.Biz;
using RcrsCore.Api.IdentityServer.Admin.Dto;
using RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel.Role;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application.Entity;

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
            _bizUser = new BizUser(applicationContext, userManager);
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
                Response = roleList.Select(x => x.RoleName).ToList()
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
                Response = roleList.Select(x => x.RoleName).ToList()
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
            messageModel.Response = new List<string>();

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
                    messageModel.Response = roleNameList;
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
                        messageModel.Response = roleNameList;
                    }
                    else
                    {
                        //ユーザーにロールを追加します。
                        if (_bizUser.AddRole(userId, roleName))
                        {
                            messageModel.Msg = "OK";
                            messageModel.Success = true;
                            messageModel.Response = _bizRole.GetRoleListByUserId(userId).Select(x => x.RoleName).ToList();
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
            messageModel.Response = new List<string>();

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
                        messageModel.Response = _bizRole.GetRoleListByUserId(userId).Select(x => x.RoleName).ToList();
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
                    messageModel.Response = roleNameList;
                }
            }

            return messageModel;
        }
    }
}