using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ProjectWeb.Models.Entity;
using ProjectWeb.Utils;

namespace ProjectWeb
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _allowedroles;
        private DBContext ctx { get; } = DbConnect.instance;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            this._allowedroles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userId = AuthenticationUtil.GetUserId(httpContext.Request, httpContext.Session);
            var userRole = ctx.Users
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