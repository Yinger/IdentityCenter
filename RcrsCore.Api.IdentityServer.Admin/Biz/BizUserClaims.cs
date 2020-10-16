using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using RcrsCore.Api.IdentityServer.Admin.Dto;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.Application.Entity;

namespace RcrsCore.Api.IdentityServer.Admin.Biz
{
    //---------------------------------------------------------------
    /// <summary>
    /// ユーザーClaimsビズネスクラス
    /// </summary>
    //---------------------------------------------------------------
    public class BizUserClaims
    {
        /// <summary> APIs for managing user </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary></summary>
        private readonly ApplicationContext _applicationDbContext;

        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="applicationDbContext"></param>
        //---------------------------------------------------------------
        public BizUserClaims(UserManager<ApplicationUser> userManager, ApplicationContext applicationDbContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーのクレームを更新行います。
        /// （既存の削除して、クレームを再作成します。）
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RefreshUserClaims(ApplicationUser user)
        {
            bool result = false;

            if (RemoveUserAllClaims(user) != -1) //既存の削除
                result = AddUserClaimsAsync(user).Result.Succeeded; //クレームを再作成

            return result;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーのクレームを更新行います。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public bool RefreshUserClaimsById(string userId)
        {
            ApplicationUser user = _userManager.FindByIdAsync(userId).Result;

            return RefreshUserClaims(user);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// クレームを全削除行います。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public int RemoveUserAllClaims(ApplicationUser user)
        {
            int result = -1;
            //クレームを全取得します。
            List<IdentityUserClaim<Guid>> listClaim = _applicationDbContext.UserClaims
                                                                           .Where(x => x.UserId == user.Id)
                                                                           .ToList();

            //削除
            _applicationDbContext.UserClaims.RemoveRange(listClaim);
            result = _applicationDbContext.SaveChanges();

            return result;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// AspNetUserClaimsにclaimを追加します。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public Task<IdentityResult> AddUserClaimsAsync(ApplicationUser user)
        {
            Task<IdentityResult> result;
            List<ApplicationRole> listUserRole;
            List<Claim> claims;

            listUserRole = (from role in _applicationDbContext.Roles
                            from uRole in _applicationDbContext.UserRoles.Where(x => x.RoleId == role.Id).DefaultIfEmpty()
                            where uRole != null && uRole.UserId == user.Id
                            select role).ToList();

            claims = new List<Claim> {
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email ?? ""),
                new Claim(IdentityConst.CustomJwtClaimTypes.LgCode, user.LgCode ?? ""),
                new Claim(IdentityConst.CustomJwtClaimTypes.UserId, user.Id.ToString().ToUpper())
            };

            if (listUserRole != null && listUserRole.Count > 0)
                claims.AddRange(listUserRole.Select(s => new Claim(JwtClaimTypes.Role, s.ToString())));
            else
                claims.Add(new Claim(JwtClaimTypes.Role, IdentityConst.DefaultRole));

            result = _userManager.AddClaimsAsync(user, claims);

            return result;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーのクレーム一覧を取得します。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public List<Claim> GetUserClaims(string userId)
        {
            List<Claim> listClaim = new List<Claim>();
            ApplicationUser user = _userManager.FindByIdAsync(userId).Result;

            if (user != null)
                listClaim = (List<Claim>)_userManager.GetClaimsAsync(user).Result;

            return listClaim;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロール名更新時に、すべて既存のロール名を新ロール名に替換
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        //---------------------------------------------------------------
        public void UpdateRoleName(string oldName, string newName)
        {
            //クレームを取得します。
            List<IdentityUserClaim<Guid>> listClaim = _applicationDbContext.UserClaims
                                                                           .Where(x => (x.ClaimType.Equals(JwtClaimTypes.Role) && x.ClaimValue.Equals(oldName)))
                                                                           .ToList();
            foreach (IdentityUserClaim<Guid> claim in listClaim)
            {
                claim.ClaimValue = newName;
                _applicationDbContext.UserClaims.Update(claim);
            }

            if (listClaim.Count > 0)
                _applicationDbContext.SaveChanges();
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ロール削除時に、既存のロールクレームを削除します。
        /// </summary>
        /// <param name="roleName"></param>
        //---------------------------------------------------------------
        public void RemoveRoleName(string roleName)
        {
            Guid userId;
            int roleCnt = 0;

            //クレームを取得します。
            List<IdentityUserClaim<Guid>> listClaim = _applicationDbContext.UserClaims
                                                                           .Where(x => (x.ClaimType.Equals(JwtClaimTypes.Role) && x.ClaimValue.Equals(roleName)))
                                                                           .ToList();
            //該当ロール名のクレームを一括削除
            if (listClaim.Count > 0)
            {
                _applicationDbContext.UserClaims.RemoveRange(listClaim);
                _applicationDbContext.SaveChanges();
            }

            //一括削除後に、roleクレームを検査（ロールクレーム情報０件の場合、デファクトロールクレームを追加します。）
            foreach (IdentityUserClaim<Guid> claim in listClaim)
            {
                //ロールクレーム情報数を取得
                userId = claim.UserId;
                roleCnt = _applicationDbContext.UserClaims.Where(x => (x.UserId == userId && x.ClaimType.Equals(JwtClaimTypes.Role))).Count();

                //ロールクレーム情報０件の場合
                if (roleCnt == 0)
                {
                    IdentityUserClaim<Guid> idc = new IdentityUserClaim<Guid>();
                    idc.UserId = userId;
                    idc.ClaimType = JwtClaimTypes.Role;
                    idc.ClaimValue = IdentityConst.DefaultRole;
                    _applicationDbContext.UserClaims.AddAsync(idc);
                }
            }

            _applicationDbContext.SaveChanges();
        }
    }
}