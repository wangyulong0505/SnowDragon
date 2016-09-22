using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CommonFuns.Helper
{
    public class HttpHelper
    {
        /// <summary>
        /// 发起GET请求（采用WebClient，不支持传Cookie）
        /// </summary>
        /// <param name="strUrl">请求Url</param>
        /// <returns>返回：请求结果</returns>
        public static string HttpGet(string strUrl)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(strUrl);

            /* 另一种用法
            using (Stream reqStream = wc.OpenRead(strUrl))
            {
                using (StreamReader reader = new StreamReader(reqStream))
                {
                    return reader.ReadToEnd();
                }
            }
             * */
        }

        /// <summary>
        /// 发起GET请求（采用HttpWebRequest，支持传Cookie）
        /// </summary>
        /// <param name="strUrl">请求Url</param>
        /// <param name="cookieContainer">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <param name="strResult">返回请求结果（如果请求失败，返回异常消息）</param>
        /// <returns>返回：是否请求成功</returns>
        public static bool HttpGet(string strUrl, CookieContainer cookieContainer, out string strResult)
        {
            HttpWebRequest request = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "GET";
                request.KeepAlive = false;
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = 30000;

                if (cookieContainer != null)
                {
                    request.CookieContainer = cookieContainer;
                }

                strResult = GetResponseResult(request, cookieContainer);
            }
            catch (Exception ex)
            {
                strResult = ex.Message;
                return false;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
            }

            return true;
        }

        /// <summary>
        /// 发起POST请求（采用WebClient，不支持传Cookie）
        /// </summary>
        /// <param name="strUrl">请求Url</param>
        /// <param name="strPostData">发送的数据</param>
        /// <returns>返回：请求结果</returns>
        public static string HttpPost(string strUrl, string strPostData)
        {
            byte[] postDataBytes = Encoding.UTF8.GetBytes(strPostData);
            WebClient wc = new WebClient();

            byte[] responseData = wc.UploadData(strUrl, postDataBytes);
            string strResult = Encoding.UTF8.GetString(responseData);

            return strResult;
        }

        /// <summary>
        /// 发起Post请求（采用HttpWebRequest，支持传Cookie）
        /// </summary>
        /// <param name="strUrl">请求Url</param>
        /// <param name="strPostData">发送的数据</param>
        /// <param name="strResult">返回请求结果（如果请求失败，返回异常消息）</param>
        /// <param name="cookieContainer">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns>返回：是否请求成功</returns>
        public static bool HttpPost(string strUrl, string strPostData, CookieContainer cookieContainer, out string strResult)
        {
            byte[] postBytes = Encoding.UTF8.GetBytes(strPostData);

            return HttpPost(strUrl, postBytes, cookieContainer, out strResult);
        }

        /// <summary>
        /// 发起Post请求（采用HttpWebRequest，支持传Cookie）
        /// </summary>
        /// <param name="strUrl">请求Url</param>
        /// <param name="formData">发送的表单数据</param>
        /// <param name="strResult">返回请求结果（如果请求失败，返回异常消息）</param>
        /// <param name="cookieContainer">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns>返回：是否请求成功</returns>
        public static bool HttpPost(string strUrl, Dictionary<string, string> formData, CookieContainer cookieContainer, out string strResult)
        {
            string strPostData = null;
            if (formData != null && formData.Count > 0)
            {
                StringBuilder sbData = new StringBuilder();
                int i = 0;
                foreach (KeyValuePair<string, string> data in formData)
                {
                    if (i > 0)
                    {
                        sbData.Append("&");
                    }
                    sbData.AppendFormat("{0}={1}", data.Key, data.Value);
                    i++;
                }
                strPostData = sbData.ToString();
            }

            byte[] postBytes = string.IsNullOrEmpty(strPostData) ? new byte[0] : Encoding.UTF8.GetBytes(strPostData);

            return HttpPost(strUrl, postBytes, cookieContainer, out strResult);
        }

        /// <summary>
        /// 发起Post请求（采用HttpWebRequest，支持传Cookie）
        /// </summary>
        /// <param name="strUrl">请求Url</param>
        /// <param name="postBytes">发送的字节数据</param>
        /// <param name="strResult">返回请求结果（如果请求失败，返回异常消息）</param>
        /// <param name="cookieContainer">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns>返回：是否请求成功</returns>
        public static bool HttpPost(string strUrl, byte[] postBytes, CookieContainer cookieContainer, out string strResult)
        {
            HttpWebRequest request = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "POST";
                request.KeepAlive = false;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postBytes.Length;
                request.Timeout = 30000;
                request.ServicePoint.Expect100Continue = false;
                if (cookieContainer != null)
                {
                    request.CookieContainer = cookieContainer;
                }

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(postBytes, 0, postBytes.Length);

                    strResult = GetResponseResult(request, cookieContainer);
                }
            }
            catch (Exception ex)
            {
                strResult = ex.Message;
                return false;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
            }

            return true;
        }

        /// <summary>
        /// 发起POST文件请求（采用WebClient，不支持传Cookie）
        /// </summary>
        /// <param name="strUrl">请求Url</param>
        /// <param name="strFilePath">发送的文件路径</param>
        /// <returns>返回：请求结果</returns>
        public static string HttpPostFile(string strUrl, string strFilePath)
        {
            WebClient wc = new WebClient();

            byte[] responseData = wc.UploadFile(strUrl, strFilePath);
            string strResult = Encoding.UTF8.GetString(responseData);

            return strResult;
        }

        /// <summary>
        /// 发起Post文件请求（采用HttpWebRequest，支持传Cookie）
        /// </summary>
        /// <param name="strUrl">请求Url</param>
        /// <param name="strFilePostName">上传文件的PostName</param>
        /// <param name="strFilePath">上传文件路径</param>
        /// <param name="cookieContainer">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <param name="strResult">返回请求结果（如果请求失败，返回异常消息）</param>
        /// <returns>返回：是否请求成功</returns>
        public static bool HttpPostFile(string strUrl, string strFilePostName, string strFilePath, CookieContainer cookieContainer, out string strResult)
        {
            HttpWebRequest request = null;
            FileStream fileStream = FileHelper.GetFileStream(strFilePath);

            try
            {
                if (fileStream == null)
                {
                    throw new FileNotFoundException();
                }

                request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "POST";
                request.KeepAlive = false;
                request.Timeout = 30000;
                request.ServicePoint.Expect100Continue = false;

                if (cookieContainer != null)
                {
                    request.CookieContainer = cookieContainer;
                }

                string boundary = string.Format("---------------------------{0}", DateTime.Now.Ticks.ToString("x"));

                byte[] header = Encoding.UTF8.GetBytes(string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: application/octet-stream\r\n\r\n",
                    boundary, strFilePostName, Path.GetFileName(strFilePath)));
                byte[] footer = Encoding.UTF8.GetBytes(string.Format("\r\n--{0}--\r\n", boundary));

                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                request.ContentLength = header.Length + fileStream.Length + footer.Length;

                using (Stream reqStream = request.GetRequestStream())
                {
                    // 写入分割线及数据信息
                    reqStream.Write(header, 0, header.Length);

                    // 写入文件
                    FileHelper.WriteFile(reqStream, fileStream);

                    // 写入尾部
                    reqStream.Write(footer, 0, footer.Length);
                }

                strResult = GetResponseResult(request, cookieContainer);
            }
            catch (Exception ex)
            {
                strResult = ex.Message;
                return false;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }

            return true;
        }


        /// <summary>
        /// 获取请求结果字符串
        /// </summary>
        /// <param name="request">请求对象（发起请求之后）</param>
        /// <param name="cookieContainer">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns>返回请求结果字符串</returns>
        private static string GetResponseResult(HttpWebRequest request, CookieContainer cookieContainer = null)
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (cookieContainer != null)
                {
                    response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
                }
                using (Stream rspStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(rspStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// 封装System.Web.HttpUtility.HtmlEncode
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlEncode(string html)
        {
            return HttpUtility.HtmlEncode(html);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.HtmlDecode
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlDecode(string html)
        {
            return HttpUtility.HtmlDecode(html);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.UrlEncode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.UrlDecode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }
    }
}
