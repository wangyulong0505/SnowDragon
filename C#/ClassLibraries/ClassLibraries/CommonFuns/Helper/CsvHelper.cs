using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CommonFuns.Helper
{
    public class CsvHelper
    {
        /// <summary>
        /// 根据DataTable生成Csv文件
        /// </summary>
        /// <param name="dtData">数据表</param>
        /// <param name="colTitles">表头列标题</param>
        /// <returns>返回Csv文件物理路径（生成失败时返回null）</returns>
        public static void ExportCSVFile(string fullFileName, DataTable dt, string[] colTitles)
        {
            HttpResponse response = HttpContext.Current.Response;
            try
            {
                StringBuilder sbHeader = new StringBuilder();
                StringBuilder sbContent = new StringBuilder();
                DateTime tempDateTime = DateTime.MinValue;
                string tempVal = "";
                for (int i = 0; i < colTitles.Length; i++)
                {
                    sbHeader.AppendFormat("{0},", colTitles[i]);
                }
                foreach (DataRow row in dt.Rows)
                {
                    StringBuilder sbRow = new StringBuilder();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j > 0)
                        {
                            sbRow.Append(",");
                        }
                        tempVal = row[j].ToString();
                        if (DateTime.TryParse(tempVal, out tempDateTime))
                        {
                            tempVal = tempDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        sbContent.AppendFormat("{0},", FilterCSVCell(tempVal));
                    }
                    sbContent.Remove(sbContent.Length - 1, 1);
                    sbContent.AppendLine();
                }
                /* 将Table的表头设为CVS文件的表头
                for (int i = 0, len = dt.Rows.Count; i < len; i++)
                {
                    for (int j = 0, len2 = dt.Columns.Count; j < len2; j++)
                    {
                        if (i == 0)
                        {
                            sbHeader.AppendFormat("{0},", dt.Columns[j].ColumnName);
                        }
                        tempVal = dt.Rows[i][j].ToString();
                        if (DateTime.TryParse(tempVal, out tempDateTime))
                            tempVal = tempDateTime.ToString("dd-MM-yyyy HH:mm:ss");
                        sbContent.AppendFormat("{0},", FilterCSVCell(tempVal));
                    }
                    sbContent.Remove(sbContent.Length - 1, 1);
                    sbContent.AppendLine();
                }
                */
                sbHeader.Remove(sbHeader.Length - 1, 1);
                sbHeader.AppendLine();
                string strContent = sbHeader.ToString() + sbContent.ToString();

                response.Buffer = true;
                response.Clear();
                response.Charset = Encoding.Default.BodyName;
                response.ContentEncoding = Encoding.GetEncoding("GB2312"); //GB2312用Excel打开时，没有乱码。
                response.AppendHeader("Content-Disposition", "attachment;filename=" + fullFileName);
                response.ContentType = "application/ms-excel";
                response.Output.Write(strContent);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Export CSV file have a error: {0}", fullFileName), ex);
            }
        }

        public static string FilterCSVCell(string cellContent)
        {
            bool isAddFlag = false;
            if (cellContent.IndexOf("\"") != -1)
            {
                cellContent = cellContent.Replace("\"", "\"\"");
                cellContent = "\"" + cellContent + "\"";
                isAddFlag = true;
            }
            if (cellContent.IndexOf(",") != -1 && isAddFlag != true)
            {
                cellContent = "\"" + cellContent + "\"";
            }
            return cellContent;
        }
    }
}
