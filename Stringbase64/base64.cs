using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAM.Stringbase64
{
    class base64
    {
        ///编码
        public static string EncodeBase64(string code)
        {
               string encode = "";
            //  byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(byteArray);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        ///解码
        public static string DecodeBase64(string code)
        {
           // string decode = "";
         //   byte[] bytes = Convert.FromBase64String(code);
            
                code = Encoding.Default.GetString(Convert.FromBase64String(code));
                return code;

            
    
        }
    }
}
