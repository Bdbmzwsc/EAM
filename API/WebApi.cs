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
        public static string GetHttpResponse(string url,string method,string user_agent,string contype)//Send GET
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            //    request.ContentType = "text/html;charset=UTF-8";
            request.ContentType = contype;
            request.UserAgent = user_agent;
            request.CookieContainer = new CookieContainer();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch(Exception ex)
            {
                return "Error:" + ex.Message;
            }
           
        }
    }
}
