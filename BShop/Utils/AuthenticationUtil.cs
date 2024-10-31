using System;
using System.Web;

namespace BShop.Utils
{
    public static class AuthenticationUtil
    {
        public static int GetUserId(HttpRequestBase request, HttpSessionStateBase session)
        {
            var userIdStr = Convert.ToString(session[Constant.USER_ID]);
            if (string.IsNullOrEmpty(userIdStr))
            {
                userIdStr = request.Cookies[Constant.USER_ID]?.Value;
            }

            return string.IsNullOrEmpty(userIdStr) ? -1 : Convert.ToInt32(userIdStr);
        }
    }
}