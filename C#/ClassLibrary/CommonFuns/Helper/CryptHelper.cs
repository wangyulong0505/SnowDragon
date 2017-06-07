using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace CommonFuns.Helper
{
    public class CryptHelper
    {
        #region 对用户密码进行加密

        /// <summary>
        /// 对用户密码进行加密，方法过时不再使用
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        /// 
        [Obsolete]
        public static string EncryptSha1(string strContent)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(strContent, "sha1");
        }

        /// <summary>
        /// 对用户密码进行加密，方法过时不再使用
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        /// 
        [Obsolete]
        public static string EncryptMd5(string strContent)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(strContent, "md5");
        }

        /// <summary>
        /// 对用户密码进行Sha1加密
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns>返回32位16进制数字字母，若要获取16位需要从32位中取16位</returns>
        public static string NewEncryptSha1(string strContent)
        {
            SHA1 sha1Hash = SHA1.Create();
            byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(strContent));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// 对用户密码进行Md5加密
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns>返回32位16进制数字字母，若要获取16位需要从32位中取16位</returns>
        public static string NewEncryptMd5(string strContent)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(strContent));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        #endregion

        #region 对文本进行普通加密

        /// <summary>
        /// 对文本进行普通加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns></returns>
        public static string Encrypt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            string password = "http://www.google.com";
            int passIndex = 0;
            int passLength = password.Length;
            int num = 0;
            int byt = 0;
            int len = str.Length;
            string resultStr = "";
            for (int i = 0; i < len; i++)
            {
                int code = (int)str[i];
                if (code >= 2048)
                {
                    byt = (byt << 24) + (((code >> 12) | 0xe0) << 16) + ((((code & 0xfff) >> 6) | 0x80) << 8) + ((code & 0x3f) | 0x80);
                    num += 24;
                }
                else if (code >= 128)
                {
                    byt = (byt << 16) + (((code >> 6) | 0xc0) << 8) + ((code & 0x3f) | 0x80);
                    num += 16;
                }
                else
                {
                    num += 8;
                    byt = (byt << 8) + code;
                }
                while (num >= 6)
                {
                    int b = byt >> (num - 6);
                    byt = byt - (b << (num - 6));
                    num -= 6;

                    b = (b + (int)password[passIndex++]) % 64;
                    passIndex = passIndex % passLength;

                    int code1 = (b <= 9) ? (b + 48) : ((b <= 35) ? (b + 55) : ((b <= 61) ? (b + 61) : ((b == 62) ? 44 : 95)));
                    resultStr += (char)code1;
                }
            }
            if (num > 0)
            {
                int b = byt << (6 - num);

                b = (b + (int)password[passIndex++]) % 64;
                passIndex = passIndex % passLength;


                resultStr += (char)((b <= 9) ? (b + 48) : ((b <= 35) ? (b + 55) : ((b <= 61) ? (b + 61) : ((b == 62) ? 44 : 95))));
            }
            return resultStr;
        }

        /// <summary>
        /// 对普通加密的文本进行解密
        /// </summary>
        /// <param name="str">要解密的字符串</param>
        /// <returns></returns>
        public static string Decrypt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            string password = "http://www.google.com";
            int passIndex = 0;
            int passLength = password.Length;
            int num = 0, byt = 0;
            int len = str.Length;
            string resultStr = "";
            int preNum = -1;
            int preIndex = 0;
            for (int i = 0; i < len; i++)
            {
                int code = (int)str[i];
                code = (code == 95) ? 63 : ((code == 44) ? 62 : ((code >= 97) ? (code - 61) : ((code >= 65) ? (code - 55) : (code - 48))));

                code = (code - (int)password[passIndex++] + 128) % 64;
                passIndex = passIndex % passLength;

                byt = (byt << 6) + code;
                num += 6;
                while (num >= 8)
                {
                    int b = byt >> (num - 8);
                    if (preIndex > 0)
                    {
                        preNum = (preNum << 6) + (b & (0x3f));
                        preIndex--;
                        if (preIndex == 0)
                        {
                            resultStr += (char)preNum;
                        }
                    }
                    else
                    {
                        if (b >= 224)
                        {
                            preNum = b & (0xf);
                            preIndex = 2;
                        }
                        else if (b >= 128)
                        {
                            preNum = b & (0x1f);
                            preIndex = 1;
                        }
                        else
                        {
                            resultStr += (char)b;
                        }
                    }
                    byt = byt - (b << (num - 8));
                    num -= 8;
                }
            }

            return resultStr;
        }

        #endregion

        #region Url加密解密

        /// <summary>
        /// Url解密
        /// </summary>
        /// <param name="reqData"></param>
        public static string UrlDecode(string strValue, bool bComplex = false)
        {
            if (string.IsNullOrEmpty(strValue))
            {
                return strValue;
            }
            if (bComplex)
            {
                string password = "http://www.google.com";
                int passIndex = 0;
                int passLength = password.Length;
                int num = 0, byt = 0;
                int len = strValue.Length;
                string resultStr = "";
                int preNum = -1;
                int preIndex = 0;

                for (int i = 0; i < len; i++)
                {
                    int code = (int)strValue[i];
                    code = (code == 95) ? 63 : ((code == 44) ? 62 : ((code >= 97) ? (code - 61) : ((code >= 65) ? (code - 55) : (code - 48))));

                    code = (code - (int)password[passIndex++] + 128) % 64;
                    passIndex = passIndex % passLength;

                    byt = (byt << 6) + code;
                    num += 6;

                    while (num >= 8)
                    {
                        int b = byt >> (num - 8);
                        if (preIndex > 0)
                        {
                            preNum = (preNum << 6) + (b & (0x3f));
                            preIndex--;
                            if (preIndex == 0)
                            {
                                resultStr += (char)preNum;
                            }
                        }
                        else
                        {
                            if (b >= 224)
                            {
                                preNum = b & (0xf);
                                preIndex = 2;
                            }
                            else if (b >= 128)
                            {
                                preNum = b & (0x1f);
                                preIndex = 1;
                            }
                            else
                            {
                                resultStr += (char)b;
                            }
                        }
                        byt = byt - (b << (num - 8));
                        num -= 8;
                    }
                }

                return resultStr;
            }
            return HttpUtility.UrlDecode(strValue);
        }

        /// <summary>
        /// Url加密
        /// </summary>
        /// <param name="strUrl"></param>
        public static string UrlEncode(string strValue, bool bComplex = false)
        {
            if (string.IsNullOrEmpty(strValue))
            {
                return strValue;
            }
            if (bComplex)
            {
                string password = "http://www.google.com";
                int passIndex = 0;
                int passLength = password.Length;
                int num = 0;
                int byt = 0;
                int len = strValue.Length;
                string resultStr = "";

                for (int i = 0; i < len; i++)
                {
                    int code = (int)strValue[i];
                    if (code >= 2048)
                    {
                        byt = (byt << 24) + (((code >> 12) | 0xe0) << 16) + ((((code & 0xfff) >> 6) | 0x80) << 8) + ((code & 0x3f) | 0x80);
                        num += 24;
                    }
                    else if (code >= 128)
                    {
                        byt = (byt << 16) + (((code >> 6) | 0xc0) << 8) + ((code & 0x3f) | 0x80);
                        num += 16;
                    }
                    else
                    {
                        num += 8;
                        byt = (byt << 8) + code;
                    }

                    while (num >= 6)
                    {
                        int b = byt >> (num - 6);
                        byt = byt - (b << (num - 6));
                        num -= 6;

                        b = (b + (int)password[passIndex++]) % 64;
                        passIndex = passIndex % passLength;

                        int code1 = (b <= 9) ? (b + 48) : ((b <= 35) ? (b + 55) : ((b <= 61) ? (b + 61) : ((b == 62) ? 44 : 95)));
                        resultStr += (char)code1;
                    }
                }

                if (num > 0)
                {
                    int b = byt << (6 - num);

                    b = (b + (int)password[passIndex++]) % 64;
                    passIndex = passIndex % passLength;


                    resultStr += (char)((b <= 9) ? (b + 48) : ((b <= 35) ? (b + 55) : ((b <= 61) ? (b + 61) : ((b == 62) ? 44 : 95))));
                }

                return resultStr;
            }
            return HttpUtility.UrlEncode(strValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqData"></param>
        public static void UrlDecode(object reqData)
        {
            foreach (PropertyInfo p in reqData.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    p.SetValue(reqData, UrlDecode(Convert.ToString(p.GetValue(reqData))), null);
                }
            }
        }

        #endregion

        #region AES加密

        public static string EncryptText(string input, string key)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            keyBytes = SHA256.Create().ComputeHash(keyBytes);
            byte[] bytesEncrypted = AESEncryptBytes(inputBytes, keyBytes);
            string result = Convert.ToBase64String(bytesEncrypted);
            return result;
        }

        private static byte[] AESEncryptBytes(byte[] inputBytes, byte[] keyBytes)
        {
            byte[] encryptedBytes = null;

            var saltBytes = new byte[9] { 13, 34, 27, 67, 189, 255, 104, 219, 122 };

            using (var ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(keyBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(32);
                    AES.IV = key.GetBytes(16);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        cs.Write(inputBytes, 0, inputBytes.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        #endregion

        #region AES解密

        public static string DecryptText(string input, string key)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AESDecryptBytes(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }

        public static byte[] AESDecryptBytes(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            var saltBytes = new byte[9] { 13, 34, 27, 67, 189, 255, 104, 219, 122 };

            using (var ms = new MemoryStream())
            {
                using (var AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(32);
                    AES.IV = key.GetBytes(16);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        #endregion
    }
}
