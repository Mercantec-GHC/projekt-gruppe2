using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazorApp.Models;

namespace BlazorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("signin")]
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
                var authProperties = new AuthenticationProperties()
                {
                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60), //Testing needed!
                    IsPersistent = value.Remember,
                    //AllowRefresh = true,
                };
                //authProperties.IsPersistent = true;

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return this.Ok();
            }

            return this.BadRequest();
        }

        [HttpPost]
        [Route("signout")]
        public async Task<ActionResult> SignOutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.Ok();
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> RegisterPost(RegisterData value)
        {
            ModelList<User> users = await BlazorApp.Models.User.QueryBy("OR", ("email", value.Email), ("username", value.Username));
            
            if (!users.Any())
            {
                User newUser = new User
                {
                    Email = value.Email,
                    FirstName = value.FirstName,
                    LastName = value.LastName,
                    Username = value.Username,
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword(value.Password, 13),
                    Role = "user",
                    CreatedAt = DateTime.UtcNow
                };
                await newUser.Commit();
                return this.Ok();
            }

            return this.BadRequest();
        }

        [HttpGet]
        [Route("access-denied")]
        public async Task<ActionResult> AccessDenied()
        {
            return this.Redirect("/");
        }
    }

    public class SigninData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }

    public class RegisterData
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
