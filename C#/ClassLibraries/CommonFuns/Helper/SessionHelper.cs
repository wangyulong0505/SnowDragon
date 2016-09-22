using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CommonFuns.Helper
{
    public class SessionHelper
    {
        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="strKey">Session名</param>
        /// <param name="obj">要保存的对象</param>
        public static void SetSession(string strKey, object obj)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[strKey] = obj;
            }
        }

        /// <summary>
        /// 从Session获取保存的对象
        /// </summary>
        /// <param name="strKey">Session名</param>
        /// <returns></returns>
        public static object GetSessionValue(string strKey)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session[strKey] != null)
            {
                return HttpContext.Current.Session[strKey];
            }

            return null;
        }

        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="strKey">Session名</param>
        public static void DeleteSession(string strKey)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Remove(strKey);
            }
        }

        /// <summary>
        /// 清空Session
        /// </summary>
        public static void ClearSession()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Clear();
            }
        }
    }
}
