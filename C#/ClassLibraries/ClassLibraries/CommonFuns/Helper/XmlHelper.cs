using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CommonFuns.Helper
{
    public class XmlHelper
    {
        /// <summary>
        /// 将字符串反序列化为Xml对象
        /// </summary>
        /// <typeparam name="T">Xml对象类型（如XmlDocument、XDocument、XmlNode、XNode等Xml对象）</typeparam>
        /// <param name="strXml">Xml字符串</param>
        /// <returns>返回：Xml对象</returns>
        public static object Deserialize<T>(string strXml)
        {
            try
            {
                using (StringReader reader = new StringReader(strXml))
                {
                    XmlSerializer des = new XmlSerializer(typeof(T));
                    return des.Deserialize(reader);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将字节流反序列化为Xml对象
        /// </summary>
        /// <typeparam name="T">Xml对象类型（如XmlDocument、XDocument、XmlNode、XNode等Xml对象）</typeparam>
        /// <param name="stream">字节流</param>
        /// <returns>返回：Xml对象</returns>
        public static object Deserialize<T>(Stream stream)
        {
            XmlSerializer des = new XmlSerializer(typeof(T));
            return des.Deserialize(stream);
        }

        /// <summary>
        /// 将Xml对象序列化为字符串
        /// </summary>
        /// <typeparam name="T">Xml对象类型（如XmlDocument、XDocument、XmlNode、XNode等Xml对象）</typeparam>
        /// <param name="obj">Xml对象</param>
        /// <returns></returns>
        public static string Serializer<T>(T obj)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    ser.Serialize(stream, obj);
                    stream.Position = 0;
                    StreamReader reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Xml文件路径获取Xml文档对象
        /// </summary>
        /// <param name="strXmlPath">Xml文件路径</param>
        /// <returns></returns>
        public static XmlDocument GetXmlDoc(string strXmlPath)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(strXmlPath);
            }
            catch (Exception ex)
            {
                //LogHelper.WriteExceptionLog(ex);
                return null;
            }

            return doc;
        }

        /// <summary>
        /// 根据Xml文件路径和XPath语句获取第一个节点
        /// </summary>
        /// <param name="strXmlPath">Xml文件路径</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static XmlNode GetNode(string strXmlPath, string strXPath)
        {
            XmlDocument doc = GetXmlDoc(strXmlPath);
            if (doc != null)
            {
                return doc.SelectSingleNode(strXPath);
            }
            return null;
        }

        /// <summary>
        /// 获取Xml文档中符合指定XPath的第一个子节点
        /// </summary>
        /// <param name="doc">Xml文档对象</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlDocument doc, string strXPath)
        {
            return doc.SelectSingleNode(strXPath);
        }

        /// <summary>
        /// 获取节点的子节点中符合指定XPath的第一个节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlNode node, string strXPath)
        {
            return node.SelectSingleNode(strXPath);
        }

        /// <summary>
        /// 根据Xml文件路径和XPath语句获取子节点集合
        /// </summary>
        /// <param name="strXmlPath">Xml文件路径</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static XmlNodeList GetNodes(string strXmlPath, string strXPath)
        {
            XmlDocument doc = GetXmlDoc(strXmlPath);
            if (doc != null)
            {
                return doc.SelectNodes(strXPath);
            }
            return null;
        }

        /// <summary>
        /// 获取Xml文档中符合指定XPath的子节点集合
        /// </summary>
        /// <param name="doc">Xml文档对象</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static XmlNodeList GetNodes(XmlDocument doc, string strXPath)
        {
            return doc.SelectNodes(strXPath);
        }

        /// <summary>
        /// 获取节点的子节点中符合指定XPath的节点集合
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static XmlNodeList GetNodes(XmlNode node, string strXPath)
        {
            return node.SelectNodes(strXPath);
        }

        /// <summary>
        /// 根据Xml文件路径和XPath语句获取第一个节点的文本
        /// </summary>
        /// <param name="strXmlPath">Xml文件路径</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static string GetNodeText(string strXmlPath, string strXPath)
        {
            XmlNode nd = GetNode(strXmlPath, strXPath);
            if (nd != null)
            {
                return nd.InnerText;
            }

            return null;
        }

        /// <summary>
        /// 获取Xml文档中符合指定XPath的第一个节点的文本
        /// </summary>
        /// <param name="doc">Xml文档对象</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static string GetNodeText(XmlDocument doc, string strXPath)
        {
            XmlNode nd = GetNode(doc, strXPath);
            if (nd != null)
            {
                return nd.InnerText;
            }

            return null;
        }

        /// <summary>
        /// 获取节点的子节点中符合指定XPath的第一个节点的文本
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static string GetNodeText(XmlNode node, string strXPath)
        {
            XmlNode nd = GetNode(node, strXPath);
            if (nd != null)
            {
                return nd.InnerText;
            }

            return null;
        }

        /// <summary>
        /// 根据Xml文件路径和XPath语句获取子节点集合的文本集合
        /// </summary>
        /// <param name="strXmlPath">Xml文件路径</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static List<string> GetNodeTexts(string strXmlPath, string strXPath)
        {
            XmlNodeList list = GetNodes(strXmlPath, strXPath);

            return ToList(list);
        }

        /// <summary>
        /// 获取Xml文档中符合指定XPath的子节点集合的文本集合
        /// </summary>
        /// <param name="doc">Xml文档对象</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static List<string> GetNodeTexts(XmlDocument doc, string strXPath)
        {
            XmlNodeList list = GetNodes(doc, strXPath);

            return ToList(list);
        }

        /// <summary>
        /// 获取节点的子节点中符合指定XPath的节点集合的文本集合
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="strXPath">XPath语句</param>
        /// <returns></returns>
        public static List<string> GetNodeTexts(XmlNode node, string strXPath)
        {
            XmlNodeList list = GetNodes(node, strXPath);

            return ToList(list);
        }

        /// <summary>
        /// 将节点集合转为文本集合
        /// </summary>
        /// <param name="list">节点集合</param>
        /// <returns></returns>
        private static List<string> ToList(XmlNodeList list)
        {
            if (list != null)
            {
                List<string> result = new List<string>();
                foreach (XmlNode node in list)
                {
                    result.Add(node.InnerText);
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// 获取节点的指定属性的属性值
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="strAttrName">属性名</param>
        /// <returns></returns>
        public static string GetAttributeValue(XmlNode node, string strAttrName)
        {
            XmlAttribute attr = node.Attributes[strAttrName];
            if (attr != null)
            {
                return attr.Value;
            }

            return null;
        }
    }
}
