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
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            //IsPersistent = true,
        };

        [HttpPost]
        [Route("api/auth/signin")]
        public async Task<ActionResult> SignInPost(SigninData value)
        {
            ModelList<User> users = await BlazorApp.Models.User.QueryBy<User>(("id", 0));
            User user = users.FirstOrDefault();
            if (user == null) return this.BadRequest();

            //string passwordHash = "$2a$13$PW8.PiOVhxsgWC9nnQgshuX0OBEdKDmsIWcFXD19Whlim5xkwwTAm";
            if (value.Email == user.Email && BCrypt.Net.BCrypt.EnhancedVerify(value.Password, user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
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
    }

    public class SigninData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
