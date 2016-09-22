using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFuns.Helper
{
    public class FileHelper
    {
        /// <summary>
        /// 根据完整文件路径获取FileStream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileStream GetFileStream(string strFileName)
        {
            FileStream fileStream = null;
            if (!string.IsNullOrEmpty(strFileName) && File.Exists(strFileName))
            {
                fileStream = new FileStream(strFileName, FileMode.Open);
            }

            return fileStream;
        }

        /// <summary>
        /// 将文件流写入请求字节流
        /// </summary>
        /// <param name="postStream">请求字节流</param>
        /// <param name="fileStream">文件流</param>
        public static void WriteFile(Stream reqStream, FileStream fileStream)
        {
            if (fileStream != null)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    reqStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
