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
        private static string ip = "";
        private static string name = "";
        private static string url = "";

        private static int cpu;
        private static int mem;
        private static int n = 0;

        public static void Init(string ip, string name)
        {
            Sender.ip = ip;
            Sender.name = name;
            url = @"http://" + ip + @"/Receive/";
        }

        public static void Send()
        {
            n++;
            if (n == 10)
            {
                SysPerfomance.GetPerformance(out cpu, out mem);
                n = 0;
            }
            
            //var param = Encoding.UTF8.GetBytes("?name=" + name + "&cpu=" + cpu + "&memory=" + mem);
            //url += Convert.ToBase64String(param);
            //Console.WriteLine(url);
            //var response = new HttpClient().GetAsync(url);
            url += "?name=" + name + "&cpu=" + cpu + "&memory=" + mem;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Proxy = null;

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var code = response.StatusCode;
                    if (code != HttpStatusCode.OK)
                    {
                        WriteComment(code);
                        WriteLog(code);
                    }
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
            if (Program.IsCommentEnabled)
            {
                Console.WriteLine(DateTime.Now + " " + code);
            }
        }

        static void WriteLog(object code)
        {
            if (Program.IsLogEnabled)
            {
                Logging.Write(code.ToString());
            }
        }
    }
}
