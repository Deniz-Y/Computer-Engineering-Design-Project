using KU.Student.Starter.Infrastructure.Extensions;
using KU.Student.Starter.Infrastructure.Helpers.Authentication;
using KU.Student.Starter.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KU.Student.Starter.UI.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKeyCloakAppSettings _keyCloakAppSettings;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IKeyCloakAppSettings keyCloakAppSettings, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _keyCloakAppSettings = keyCloakAppSettings;
            _configuration = configuration;
         
        }

        [Authorize]
        public IActionResult Index()
        {
            //var roles = HttpContext.User.Claims.Where(c => c.Type == ApplicationPolicyType.KsPermission).Select(c => c.Value);
            return View();
        }



        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            var refUrl = _httpContextAccessor.HttpContext!.Request.Headers.Referer;

            return Redirect($"{_keyCloakAppSettings.Authority}/protocol/openid-connect/logout?id_token_hint={ConfigureAuthenticationServiceExtensions.GetIdToken(User.Identity)}&post_logout_redirect_uri={(refUrl.Any() ? refUrl.First() : _configuration["ApplicationUrl"])}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}