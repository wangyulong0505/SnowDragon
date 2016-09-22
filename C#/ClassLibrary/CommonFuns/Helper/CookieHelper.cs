using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CommonFuns.Helper
{
    public class CookieHelper
    {
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="strKey">Cookie名</param>
        /// <param name="obj">要保存的对象</param>
        /// <param name="dtExpires">过期时间</param>
        public static void SetCookie(string strKey, object obj, DateTime? dtExpires = null)
        {
            string strValue = new JavaScriptSerializer().Serialize(obj);

            SetCookie(strKey, strValue, dtExpires);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="strKey">Cookie名</param>
        /// <param name="strValue">要保存的字符串</param>
        /// <param name="dtExpires">过期时间</param>
        public static void SetCookie(string strKey, string strValue, DateTime? dtExpires = null)
        {
            if (dtExpires == null || !dtExpires.HasValue)
            {
                dtExpires = DateTime.Now.AddMinutes(60);
            }

            HttpCookie cookie = HttpContext.Current.Request.Cookies[strKey];
            if (cookie != null)
            {
                cookie.Value = strValue;
                cookie.Expires = dtExpires.Value;
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
            else
            {
                cookie = new HttpCookie(strKey, strValue);
                cookie.Expires = dtExpires.Value;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 从Cookie获取保存的对象
        /// </summary>
        /// <param name="strKey">Cookie名</param>
        /// <returns></returns>
        public static T GetCookieValue<T>(string strKey)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strKey];
            if (cookie != null)
            {
                return new JavaScriptSerializer().Deserialize<T>(cookie.Value);
            }

            return default(T);
        }

        /// <summary>
        /// 从Cookie获取保存的字符串
        /// </summary>
        /// <param name="strKey">Cookie名</param>
        /// <returns></returns>
        public static string GetCookieValue(string strKey)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strKey];
            if (cookie != null)
            {
                return cookie.Value;
            }

            return null;
        }

        /// <summary>
        /// 删除Cookie
        /// </summary>
        /// <param name="strKey">Cookie名</param>
        public static void DeleteCookie(string strKey)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strKey];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-2);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }

        /// <summary>
        /// 清空Cookie
        /// </summary>
        public static void ClearCookie()
        {
            foreach (HttpCookie cookie in HttpContext.Current.Response.Cookies)
            {
                cookie.Expires = DateTime.Now.AddDays(-2);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }
    }
}
