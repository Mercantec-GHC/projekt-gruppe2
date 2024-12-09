using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazorApp.Models;

namespace BlazorApp.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly AuthenticationProperties COOKIE_EXPIRES = new AuthenticationProperties()
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30), //Testing needed!
            //IsPersistent = true,
        };

        [HttpPost]
        [Route("api/auth/signin")]
        public async Task<ActionResult> SignInPost(SigninData value)
        {
            ModelList<User> users = await BlazorApp.Models.User.QueryBy(("email", value.Email));
            User user = users.FirstOrDefault();
            if (user == null) return this.BadRequest();

            if (value.Email == user.Email && BCrypt.Net.BCrypt.EnhancedVerify(value.Password, user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.PrimarySid, $"{user.Id}")
                };

                var claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = COOKIE_EXPIRES;
                authProperties.IsPersistent = true;

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return this.Ok();
            }

            return this.BadRequest();
        }

        [HttpPost]
        [Route("api/auth/signout")]
        public async Task<ActionResult> SignOutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.Ok();
        }

        [HttpGet]
        [Route("/access-denied")]
        public async Task<ActionResult> AccessDenied()
        {
            return this.Redirect("/");
        }
    }

    public class SigninData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
