using BShop.Utils;
using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace BShop
{
    public class CustomAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            var email = Convert.ToString(filterContext.HttpContext.Session[Constant.EMAIL]);
            if (string.IsNullOrEmpty(email))
            {
                // get email from cookie
                email = filterContext.HttpContext.Request.Cookies[Constant.EMAIL]?.Value;
            }
            if (string.IsNullOrEmpty(email))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                // Redirecting the user to the Login View of Account Controller
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "Index" },
                        // return area name if you have one
                        { "area", "" },
                        { "act", "login" }
                    });
            }
        }
    }
}