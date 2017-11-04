using System;
using System.CodeDom;
using System.Collections.Generic;
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
        private static HttpClient client = new HttpClient();
        private static int cpu;
        private static int mem;

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
                    Console.WriteLine("Error getting name");
                return true;
            }
                
            return false;
        }

        public static void GetName()
        {
            var response = client.GetAsync(baseUrl + "getname").Result;
            if (response.IsSuccessStatusCode)
            {
                var tname = response.Content.ReadAsStringAsync().Result;
                name = tname.Trim(charsToTrim);
            }
            
        }

        public static void Send()
        {
            SysPerfomance.GetPerformance(out cpu, out mem);
            var url = baseUrl + "set/?name=" + name + "&cpu=" + cpu + "&memory=" + mem;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
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

        public static void Send2()
        {
            SysPerfomance.GetPerformance(out cpu, out mem);
            var url = baseUrl + "set/?name=" + name + "&cpu=" + cpu + "&memory=" + mem;
            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                WriteComment(" cpu= " + cpu + " memory= " + mem + " " + response.StatusCode);
                WriteLog(" cpu= " + cpu + " memory= " + mem + " " + response.StatusCode);
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
