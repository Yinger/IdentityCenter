using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RcrsCore.IdentityServer.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}