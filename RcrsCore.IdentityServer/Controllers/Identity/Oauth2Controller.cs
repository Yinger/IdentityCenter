using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RcrsCore.IdentityServer.Extensions.IdentityServer;
using RcrsCore.IdentityServer.Models.DomainEntity;
using RcrsCore.IdentityServer.Models.Identity;

namespace RcrsCore.IdentityServer.Controllers.Identity
{
    //---------------------------------------------------------------
    /// <summary>
    /// 認証コントローラー
    /// </summary>
    //---------------------------------------------------------------
    [SecurityHeaders]
    public class Oauth2Controller : QuickStartAccountController
    {
        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="userManager">APIs for managing user</param>
        /// <param name="roleManager">APIs for managing roles</param>
        /// <param name="signInManager">APIs for user sign in</param>
        /// <param name="interaction">services be used by the user interface to communicate with IdentityServer</param>
        /// <param name="clientStore">client configuration</param>
        /// <param name="schemeProvider">managing what authenticationSchemes are supported</param>
        /// <param name="events">event service</param>
        //---------------------------------------------------------------
        public Oauth2Controller(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events) : base(userManager, roleManager, signInManager, interaction, clientStore, schemeProvider, events)
        {
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Show login page
        /// </summary>
        //---------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Authorize(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            // external providerの場合
            if (vm.IsExternalLoginOnly)
                return await ExternalLogin(vm.ExternalLoginScheme, returnUrl); // we only have one option for logging in and it's an external provider

            return View(vm);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        //---------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorize(LoginInputModel model, string button)
        {
            if (button != "login")
            {
                // the user clicked the "cancel" button
                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberLogin, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));

                        // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                        // the IsLocalUrl check is only necessary if you want to support additional local pages, otherwise IsValidReturnUrl is more strict
                        if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                            return Redirect(model.ReturnUrl);

                        return Redirect("~/");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.UserName, "invalid credentials"));

                ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }
    }
}