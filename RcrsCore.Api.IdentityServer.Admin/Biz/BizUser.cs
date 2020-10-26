using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RcrsCore.IdentityServer.Dto;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.IdentityServer.Dto.ViewModel.User;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application;

namespace RcrsCore.Api.IdentityServer.Admin.Biz
{
    //---------------------------------------------------------------
    /// <summary>
    /// ユーザービズネスクラス
    /// </summary>
    //---------------------------------------------------------------
    public class BizUser
    {
        /// <summary></summary>
        private readonly ApplicationContext _applicationContext;

        /// <summary> APIs for managing user </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary></summary>
        private readonly BizUserClaims _bizClaim;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="applicationContext"></param>
        /// <param name="userManager">APIs for managing user</param>
        //---------------------------------------------------------------
        public BizUser(ApplicationContext applicationContext, UserManager<ApplicationUser> userManager)
        {
            _applicationContext = applicationContext;
            _userManager = userManager;
            _bizClaim = new BizUserClaims(userManager, applicationContext);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// アカウント一覧を取得します。
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<UserViewModel> GetUserList(UserSearchModel searchModel = null)
        {
            List<ApplicationUser> listAppUser = (from user in _applicationContext.Users.Include(x => x.UserRoles)
                                                 select user).ToList();
            List<UserViewModel> listViewUser = new List<UserViewModel>();

            foreach (ApplicationUser appUser in listAppUser)
                listViewUser.Add(mapperEntityToViewModel(appUser));

            if (searchModel != null)
            {
                //ユーザー名
                if (!string.IsNullOrEmpty(searchModel.LoginName))
                    listViewUser = listViewUser.Where(x => x.LoginName.Contains(searchModel.LoginName)).ToList();

                //メール
                if (!string.IsNullOrEmpty(searchModel.Email))
                    listViewUser = listViewUser.Where(x => x.Email.Contains(searchModel.Email)).ToList();

                //市区町村
                if (!string.IsNullOrEmpty(searchModel.LgCode))
                    listViewUser = listViewUser.Where(x => x.LgCode.Equals(searchModel.LgCode)).ToList();

                //課
                if (!string.IsNullOrEmpty(searchModel.LgKaKakari))
                    listViewUser = listViewUser.Where(x => x.LgKaKakari.Contains(searchModel.LgKaKakari)).ToList();

                //ロール
                if (!string.IsNullOrEmpty(searchModel.RoleName))
                    listViewUser = listViewUser.Where(x => x.ListRole.Where(y => y.Contains(searchModel.RoleName)).Count() > 0).ToList();
            }

            return listViewUser;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 対応ロールのアカウント一覧を取得します。
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<UserViewModel> GetUserListByRoleName(string roleName, bool isEqual)
        {
            List<ApplicationUser> listAppUser = (from user in _applicationContext.Users.Include(x => x.UserRoles)
                                                 select user).ToList();
            List<UserViewModel> listViewUser = new List<UserViewModel>();

            foreach (ApplicationUser appUser in listAppUser)
                listViewUser.Add(mapperEntityToViewModel(appUser));

            if (isEqual)
                listViewUser = listViewUser.Where(x => x.ListRole.Where(y => y.Equals(roleName)).Count() > 0).ToList();
            else
                listViewUser = listViewUser.Where(x => x.ListRole.Where(y => y.Contains(roleName)).Count() > 0).ToList();

            return listViewUser;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーIDでユーザーを取得します。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public UserViewModel FindById(string userId)
        {
            Guid guid = new Guid(userId.ToUpper());
            ApplicationUser appUser = (from user in _applicationContext.Users.Include(x => x.UserRoles)
                                       where user.Id == guid
                                       select user).FirstOrDefault();

            return mapperEntityToViewModel(appUser);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザー名でユーザーを取得します。
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public UserViewModel FindByName(string userName)
        {
            ApplicationUser appUser = (from user in _applicationContext.Users.Include(x => x.UserRoles)
                                       where user.UserName.Equals(userName)
                                       select user).FirstOrDefault();
            return mapperEntityToViewModel(appUser);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// 同名チェック
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool IsNameExist(string userName)
        {
            var user = (from appUser in _applicationContext.Users
                        where appUser.UserName.ToLower().Equals(userName.ToLower())
                        select appUser).FirstOrDefault();

            if (user == null)
                return false;
            else
                return true;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーを新規します。
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public UserViewModel CreateUser(UserViewModel viewModel)
        {
            viewModel.Id = "";

            ApplicationUser appUser = new ApplicationUser();
            appUser.UserName = viewModel.LoginName;
            appUser.Email = viewModel.Email;
            appUser.LgCode = viewModel.LgCode;
            appUser.LgKaKakari = viewModel.LgKaKakari;

            if (_userManager.CreateAsync(appUser, IdentityConst.DefaultPassword).Result.Succeeded)
            {
                if (_bizClaim.AddUserClaimsAsync(appUser).Result.Succeeded)
                    viewModel = mapperEntityToViewModel(appUser);
            }

            return viewModel;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザー情報を更新行います。
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public UserViewModel EditUser(UserViewModel viewModel)
        {
            Guid userId = new Guid(viewModel.Id.ToUpper());
            ApplicationUser appUser = (from user in _applicationContext.Users.Include(x => x.UserRoles)
                                       where user.Id == userId
                                       select user).FirstOrDefault();

            if (appUser != null)
            {
                appUser.UserName = viewModel.LoginName;
                appUser.Email = viewModel.Email;
                appUser.LgCode = viewModel.LgCode;
                appUser.LgKaKakari = viewModel.LgKaKakari;

                if (_userManager.UpdateAsync(appUser).Result.Succeeded)
                    return mapperEntityToViewModel(appUser);
            }

            return null;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーを削除します。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RemoveUser(string userId)
        {
            ApplicationUser appUser = _userManager.FindByIdAsync(userId.ToUpper()).Result;

            if (appUser == null)
                return false;
            else
            {
                if (_userManager.DeleteAsync(appUser).Result.Succeeded)
                {
                    _bizClaim.RemoveUserAllClaims(appUser);
                    return true;
                }
            }
            return false;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーにロールを追加します。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool AddRole(string userId, string roleName)
        {
            ApplicationUser user = _userManager.FindByIdAsync(userId.ToUpper()).Result;
            ApplicationRole role = _applicationContext.Roles.Where(x => x.Name.Equals(roleName)).FirstOrDefault();

            if (user != null && role != null)
            {
                ApplicationUserRole userRole = new ApplicationUserRole
                {
                    Role = role,
                    User = user,
                    RoleId = role.Id,
                    UserId = user.Id
                };

                _applicationContext.UserRoles.AddAsync(userRole);
                _applicationContext.SaveChanges();
                _bizClaim.RemoveUserAllClaims(user);
                _bizClaim.AddUserClaimsAsync(user);

                return true;
            }

            return false;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーにロールを削除します。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RemoveRole(string userId, string roleName)
        {
            ApplicationUser user = _userManager.FindByIdAsync(userId.ToUpper()).Result;
            ApplicationRole role = _applicationContext.Roles.Where(x => x.Name.Equals(roleName)).FirstOrDefault();

            if (user != null && role != null)
            {
                ApplicationUserRole userRole = _applicationContext.UserRoles.Where(x => (x.UserId.ToString() == userId && x.RoleId == role.Id)).FirstOrDefault();

                if (userRole != null)
                {
                    _applicationContext.UserRoles.Remove(userRole);
                    _applicationContext.SaveChanges();

                    _bizClaim.RemoveUserAllClaims(user);
                    _bizClaim.AddUserClaimsAsync(user);

                    return true;
                }
            }

            return false;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Map ApplicationUser to UserViewModel
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        private UserViewModel mapperEntityToViewModel(ApplicationUser appUser)
        {
            UserViewModel viewUser = null;

            if (appUser != null)
            {
                List<string> listRole = new List<string>();

                if (appUser.UserRoles != null && appUser.UserRoles.Any())
                {
                    listRole = (from userRole in appUser.UserRoles
                                from role in _applicationContext.Roles.Where(x => x.Id == userRole.RoleId)
                                select role.Name).ToList();
                }

                viewUser = new UserViewModel();
                viewUser.Id = appUser.Id.ToString().ToUpper();
                viewUser.LoginName = appUser.UserName;
                viewUser.Email = appUser.Email;
                viewUser.LgCode = appUser.LgCode;
                viewUser.LgKaKakari = appUser.LgKaKakari;
                viewUser.ListRole = listRole;
                viewUser.ListClaim = _bizClaim.GetUserClaims(viewUser.Id).Select(s => s.Type).Distinct().ToList(); //roleの場合、重複ので、Distinct処理
            }

            return viewUser;
        }
    }
}