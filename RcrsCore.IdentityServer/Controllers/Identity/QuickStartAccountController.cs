using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;
using RcrsCore.IdentityServer.Models.Identity;

namespace RcrsCore.IdentityServer.Controllers.Identity
{
    //---------------------------------------------------------------
    /// <summary>
    /// IdentityServerのQuickStartの共通コントローラー
    /// </summary>
    //---------------------------------------------------------------
    public class QuickStartAccountController : Controller
    {
        /// <summary>APIs for managing user</summary>
        public readonly UserManager<ApplicationUser> _userManager;

        /// <summary>APIs for managing roles</summary>
        public readonly RoleManager<ApplicationRole> _roleManager;

        /// <summary>APIs for user sign in</summary>
        public readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>services be used by the user interface to communicate with IdentityServer</summary>
        public readonly IIdentityServerInteractionService _interaction;

        /// <summary>client configuration</summary>
        public readonly IClientStore _clientStore;

        /// <summary>managing what authenticationSchemes are supported</summary>
        public readonly IAuthenticationSchemeProvider _schemeProvider;

        /// <summary>event service</summary>
        public readonly IEventService _events;

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
        public QuickStartAccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        //---------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                // windows authentication needs special handling
                return await processWindowsLoginAsync(returnUrl);
            }
            else
            {
                // start challenge and roundtrip the return URL and
                var props = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("ExternalLoginCallback"),
                    Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                    }
                };
                return Challenge(props, provider);
            }
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        //---------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = await findUserFromExternalProviderAsync(result);
            if (user == null)
            {
                // this might be where you might initiate a custom workflow for user registration
                // in this sample we don't show how that would be done, as our sample implementation
                // simply auto-provisions new external user
                user = await autoProvisionUserAsync(provider, providerUserId, claims);
            }

            // this allows us to collect any additonal claims or properties
            // for the specific prtotocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            processLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            processLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            processLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);
            var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id.ToString();
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id.ToString(), name));
            await HttpContext.SignInAsync(user.Id.ToString(), name, provider, localSignInProps, additionalLocalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // validate return URL and redirect back to authorization endpoint or a local page
            var returnUrl = result.Properties.Items["returnUrl"];
            if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        /************************************************************************************************/
        /*                   helper APIs for the AccountController & Oauth2Controller                   */
        /************************************************************************************************/
        //=======================================================================================================================-

        //---------------------------------------------------------------
        /// <summary>
        /// LoginViewModelを作ります。
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            // Idpの場合(The external identity provider requested)
            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    UserName = context?.LoginHint,
                    ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                };
            }
            // Idp以外の場合
            else
            {
                var schemes = await _schemeProvider.GetAllSchemesAsync(); // get all currently registered Microsoft.AspNetCore.Authentication.AuthenticationSchemes
                var allowLocal = true;
                var providers = schemes.Where(x => x.DisplayName != null || (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase)))
                                       .Select(x => new ExternalProvider
                                       {
                                           DisplayName = x.DisplayName,
                                           AuthenticationScheme = x.Name
                                       }).ToList();

                if (context?.ClientId != null)
                {
                    var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                    if (client != null)
                    {
                        allowLocal = client.EnableLocalLogin;

                        if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                            providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }

                return new LoginViewModel
                {
                    AllowRememberLogin = AccountOptions.AllowRememberLogin,
                    EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                    ReturnUrl = returnUrl,
                    UserName = context?.LoginHint,
                    ExternalProviders = providers.ToArray()
                };
            }
        }

        //---------------------------------------------------------------
        /// <summary>
        /// LoginViewModelを作ります。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.UserName = model.UserName;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// LogoutViewModelを作ります。
        /// </summary>
        /// <param name="logoutId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// LogoutViewModelを作ります。
        /// </summary>
        /// <param name="logoutId"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        public async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// windows login
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        private async Task<IActionResult> processWindowsLoginAsync(string returnUrl)
        {
            //// see if windows auth has already been requested and succeeded
            var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            //if (result?.Principal is WindowsPrincipal wp)
            //{
            //    // we will issue the external cookie and then redirect the
            //    // user back to the external callback, in essence, tresting windows
            //    // auth the same as any other external authentication mechanism
            //    var props = new AuthenticationProperties()
            //    {
            //        RedirectUri = Url.Action("ExternalLoginCallback"),
            //        Items =
            //        {
            //            { "returnUrl", returnUrl },
            //            { "scheme", AccountOptions.WindowsAuthenticationSchemeName },
            //        }
            //    };

            //    var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
            //    id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
            //    id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

            //    // add the groups as claims -- be careful if the number of groups is too large
            //    if (AccountOptions.IncludeWindowsGroups)
            //    {
            //        var wi = wp.Identity as WindowsIdentity;
            //        var groups = wi.Groups.Translate(typeof(NTAccount));
            //        var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
            //        id.AddClaims(roles);
            //    }

            //    await HttpContext.SignInAsync(
            //        IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //        new ClaimsPrincipal(id),
            //        props);
            //    return Redirect(props.RedirectUri);
            //}
            //else
            //{
            //    // trigger windows auth
            //    // since windows auth don't support the redirect uri,
            //    // this URL is re-triggered when we call challenge
            //    return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
            //}

            return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// find external user
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)> findUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ?? externalUser.FindFirst(ClaimTypes.NameIdentifier) ?? throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await _userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        //---------------------------------------------------------------
        /// <summary>
        /// ユーザーの情報を作成します。
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="providerUserId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        //---------------------------------------------------------------
        private async Task<ApplicationUser> autoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            // user's display name
            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null)
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            else
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                if (first != null && last != null)
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                else if (first != null)
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                else if (last != null)
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
            }

            // email
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email != null)
                filtered.Add(new Claim(JwtClaimTypes.Email, email));

            var user = new ApplicationUser { UserName = Guid.NewGuid().ToString() };
            var identityResult = await _userManager.CreateAsync(user);

            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            if (filtered.Any())
            {
                identityResult = await _userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }

            identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            return user;
        }

        //---------------------------------------------------------------
        /// <summary>
        /// oidc callback
        /// </summary>
        /// <param name="externalResult"></param>
        /// <param name="localClaims"></param>
        /// <param name="localSignInProps"></param>
        //---------------------------------------------------------------
        private void processLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }

        //---------------------------------------------------------------
        /// <summary>
        /// WsFed callback
        /// </summary>
        /// <param name="externalResult"></param>
        /// <param name="localClaims"></param>
        /// <param name="localSignInProps"></param>
        //---------------------------------------------------------------
        private void processLoginCallbackForWsFed(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
        }

        //---------------------------------------------------------------
        /// <summary>
        /// Saml2p callback
        /// </summary>
        /// <param name="externalResult"></param>
        /// <param name="localClaims"></param>
        /// <param name="localSignInProps"></param>
        //---------------------------------------------------------------
        private void processLoginCallbackForSaml2p(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
        }
    }
}