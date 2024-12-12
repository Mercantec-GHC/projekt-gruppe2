using BlazorApp.Models;
using System.Security.Claims;

namespace BlazorApp.Service
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private User _user;
        public User CurrentUser
        {
            get
            {
                return _user ?? new User();
            }
        }

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task UpdateUser()
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                if (_user == null)
                {
                    var claims = user.Claims;
                    var id = claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid).Value;
                    ModelList<User> users = await User.QueryBy(("id", id));
                    _user = users.FirstOrDefault();
                    Console.WriteLine(id);
                }
            }
            else _user = null;
        }

        //public User GetCurrentUser()
        //{
        //    var user = _httpContextAccessor.HttpContext.User;

        //    if (user.Identity.IsAuthenticated)
        //    {
        //        return new User
        //        {
        //            UserName = user.Identity.Name,
        //            IsAuthenticated = true
        //        };
        //    }

        //    return _user;
        //}
    }

}