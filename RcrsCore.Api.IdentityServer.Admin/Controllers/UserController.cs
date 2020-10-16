using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RcrsCore.Api.IdentityServer.Admin.Biz;
using RcrsCore.Api.IdentityServer.Admin.Dto;
using RcrsCore.Api.IdentityServer.Admin.Dto.ViewModel.User;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application.Entity;

namespace RcrsCore.Api.IdentityServer.Admin.Controllers
{
    //---------------------------------------------------------------
    /// <summary>
    /// ユーザーコントローラー
    /// </summary>
    //---------------------------------------------------------------
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserController : Controller
    {
        /// <summary></summary>
        private readonly BizUser _bizUser;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="applicationContext"></param>
        //---------------------------------------------------------------
        public UserController(UserManager<ApplicationUser> userManager, ApplicationContext applicationContext)
        {
            _bizUser = new BizUser(applicationContext, userManager);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザー全件取得
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("GetAll")]
        public MessageModel<List<UserViewModel>> GetAll()
        {
            List<UserViewModel> userList = _bizUser.GetUserList();

            return new MessageModel<List<UserViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Response = userList
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// IDでユーザーを取得します。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("FindById")]
        public MessageModel<UserViewModel> FindById(string id)
        {
            return new MessageModel<UserViewModel>()
            {
                Msg = "OK",
                Success = true,
                Response = _bizUser.FindById(id)
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザー名でユーザーを取得します。
        /// </summary>
        /// <param name="name">ユーザー名（全等）</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("FindByName")]
        public MessageModel<UserViewModel> FindByName(string name)
        {
            return new MessageModel<UserViewModel>()
            {
                Msg = "OK",
                Success = true,
                Response = _bizUser.FindByName(name)
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロール名でユーザー一覧を取得します。
        /// </summary>
        /// <param name="roleName">ロール名</param>
        /// <param name="isEqual">全等フラグ</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpGet]
        [Route("GetByRole")]
        public MessageModel<List<UserViewModel>> GetByRole(string roleName, bool isEqual = false)
        {
            if (roleName == null)
                roleName = "";

            List<UserViewModel> userList = _bizUser.GetUserListByRoleName(roleName, isEqual);

            return new MessageModel<List<UserViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Response = userList
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
        public MessageModel<List<UserViewModel>> Search([FromBody] UserSearchModel condition)
        {
            List<UserViewModel> userList = _bizUser.GetUserList(condition);

            return new MessageModel<List<UserViewModel>>()
            {
                Msg = "OK",
                Success = true,
                Response = userList
            };
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 新しいアカウントを作成します。
        /// </summary>
        /// <param name="viewUser">ユーザー情報</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("Create")]
        public MessageModel<UserViewModel> Create([FromBody] UserViewModel viewUser)
        {
            var returnModel = new MessageModel<UserViewModel>();
            returnModel.Response = viewUser;

            if (string.IsNullOrEmpty(viewUser.LoginName) || string.IsNullOrEmpty(viewUser.LoginName.Trim()))
            {
                returnModel.Success = false;
                returnModel.Msg = "ユーザー名は必須項目です。";
            }
            else if (_bizUser.IsNameExist(viewUser.LoginName.Trim()))
            {
                returnModel.Success = false;
                returnModel.Msg = "ユーザー名既存しました。";
            }
            else
            {
                viewUser.LoginName = viewUser.LoginName.Trim();
                var newUser = _bizUser.CreateUser(viewUser);
                if (string.IsNullOrEmpty(newUser.Id))
                {
                    returnModel.Success = false;
                    returnModel.Msg = "新規失敗";
                }
                else
                {
                    returnModel.Success = true;
                    returnModel.Msg = "OK";
                    returnModel.Response = newUser;
                }
            }

            return returnModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーを更新します。
        /// （ユーザーのロール情報は別途で管理します）
        /// </summary>
        /// <param name="viewModel">ユーザー情報</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpPost]
        [Route("Edit")]
        public MessageModel<UserViewModel> Edit([FromBody] UserViewModel viewModel)
        {
            MessageModel<UserViewModel> messageModel = new MessageModel<UserViewModel>();
            messageModel.Response = viewModel;
            messageModel.Msg = "OK";

            if (viewModel != null)
            {
                UserViewModel user = _bizUser.FindById(viewModel.Id);

                if (user == null)
                {
                    messageModel.Success = false;
                    messageModel.Msg = "ユーザーが見つかりませんでした。";
                }
                else
                {
                    if (!user.LoginName.Equals(viewModel.LoginName))
                    {
                        if (string.IsNullOrEmpty(viewModel.LoginName) || string.IsNullOrEmpty(viewModel.LoginName.Trim()))
                        {
                            messageModel.Success = false;
                            messageModel.Msg = "ユーザー名は必須項目です。";
                        }
                        else if (_bizUser.IsNameExist(viewModel.LoginName.Trim()))
                        {
                            messageModel.Success = false;
                            messageModel.Msg = "ユーザー名既存しました。";
                        }
                    }

                    //保存行います。
                    if (messageModel.Msg.Equals("OK"))
                    {
                        messageModel.Success = true;
                        messageModel.Response = _bizUser.EditUser(viewModel);
                    }

                    //保存失敗の場合こちら
                    if (messageModel.Response == null)
                    {
                        messageModel.Success = false;
                        messageModel.Msg = "保存失敗";
                        messageModel.Response = viewModel;
                    }
                }
            }
            else
            {
                messageModel.Success = false;
                messageModel.Msg = "ユーザー情報はNULLです。";
            }

            return messageModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 削除行います。
        /// </summary>
        /// <param name="id">ユーザーId</param>
        /// <returns></returns>
        //---------------------------------------------------------------
        [HttpDelete]
        [Route("Delete")]
        public MessageModel<bool> Delete(string id)
        {
            var returnModel = new MessageModel<bool>();

            returnModel.Response = _bizUser.RemoveUser(id);

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

        //=============================================================================> role
    }
}