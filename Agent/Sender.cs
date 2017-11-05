using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Agent
{
    class Sender
    {
        private static string host = "";
        private static string name = "";
        public static string baseUrl = "";

        
        //private static HttpClient client = new HttpClient();
        public static int cpu;
        public static int mem;

        public static void Init(string host, string name)
        {
            Sender.host = host;
            Sender.name = name;
            Sender.baseUrl = @"http://" + Sender.host + @"/SampleApi/api/performances/set/";
        }
        

        public static void Send()
        {
            
            var url = baseUrl + "?name=" + name + "&cpu=" + cpu + "&memory=" + mem;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = true;
            request.Proxy = null;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var code = response.StatusCode;
                    WriteComment(" cpu= " + cpu + " memory= " + mem + " " + code);
                    WriteLog(" cpu= " + cpu + " memory= " + mem + " " + code);
                    /*if (code != HttpStatusCode.OK)
                    {
                        
                    }*/
                }


            }
            catch (Exception e)
            {
                WriteComment(e.Message);
                WriteLog(e.Message);
            }
        }
        
        static void WriteComment(object code)
        {
            if (Program.isCommentEnabled)
            {
                Console.WriteLine(DateTime.Now + " " + code);
            }
        }

        static void WriteLog(object code)
        {
            if (Program.isLogEnabled)
            {
                Logger.Write(code.ToString());
            }
        }
    }
}
