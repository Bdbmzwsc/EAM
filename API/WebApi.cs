using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;

namespace EAM.API
{
    class WebApi
    {
        public static string GetHttpResponse(string url,string method,string user_agent,string contype,string header,string content)//Send GET
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            //    request.ContentType = "text/html;charset=UTF-8";
            request.ContentType = contype;
            request.UserAgent = user_agent;
            request.CookieContainer = new CookieContainer();
            if (header!="") request.Headers.Add(header);
            if (content != "")
            {
                byte[] data = Encoding.UTF8.GetBytes(content);
                request.ContentLength = data.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
            }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;

        }
        public static string Download_file(string url, string path)//Download File
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, path);//下载文件
                    return path;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
