using System;
using System.Web;

namespace BShop.Utils
{
    public static class AuthenticationUtil
    {
        public static int GetUserId(HttpRequestBase request, HttpSessionStateBase session)
        {
            var userIdStr = Convert.ToString(session[Constant.UserId]);
            if (string.IsNullOrEmpty(userIdStr))
            {
                userIdStr = request.Cookies[Constant.UserId]?.Value;
            }

            return string.IsNullOrEmpty(userIdStr) ? -1 : Convert.ToInt32(userIdStr);
        }
    }
}