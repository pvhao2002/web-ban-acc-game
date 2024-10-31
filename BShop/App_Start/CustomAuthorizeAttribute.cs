using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BShop.Models.Entity;
using BShop.Utils;

namespace BShop
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _allowedroles;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            this._allowedroles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userId = AuthenticationUtil.GetUserId(httpContext.Request, httpContext.Session);
            var userRole = DBContext.Instance.Users
                .FirstOrDefault(u => u.UserId == userId && Constant.ACTIVE.Equals(u.Status));
            if (userRole == null) return false;
            var lowerUserRole = userRole.Role.ToLower();
            return _allowedroles.Any(role => role.ToLower().Equals(lowerUserRole));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "UnAuthorized" },
                    { "area", "" }
                });
        }
    }
}