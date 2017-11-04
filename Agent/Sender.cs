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

        private static readonly char[] charsToTrim = { '"', '\\' };
        //private static HttpClient client = new HttpClient();
        //private static int cpu;
        //private static int mem;

        public static bool Init(string host)
        {
            Sender.host = host;
            Sender.baseUrl = @"http://" + Sender.host + @"/SampleApi/api/performances/";
            Sender.GetName();
            if (name.Length > 0)
            {
                if (Program.isLogEnabled)
                    Logger.Init(name);
                if(Program.isCommentEnabled)
                    Console.WriteLine("Getting name done");
                return true;
            }
            if (Program.isCommentEnabled)
                Console.WriteLine("Error getting name done");
            return false;
        }

        public static void GetName()
        {
            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "getname");
            request.KeepAlive = true;
            request.Proxy = null;
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var code = response.StatusCode;
                    if (code == HttpStatusCode.OK)
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            name = reader.ReadToEnd().Trim(charsToTrim);
                        }
                }
            }
            catch (Exception e)
            {
                
            }
            
        }

        public static void Send()
        {
            //SysPerfomance.GetPerformance(out cpu, out mem);
            var url = baseUrl + "set/?name=" + name + "&cpu=" + SysPerfomance.cpu + "&memory=" + SysPerfomance.mem;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.KeepAlive = true;
            //request.Proxy = null;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var code = response.StatusCode;
                    WriteComment(" cpu= " + SysPerfomance.cpu + " memory= " + SysPerfomance.mem + " " + code);
                    WriteLog(" cpu= " + SysPerfomance.cpu + " memory= " + SysPerfomance.mem + " " + code);
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
