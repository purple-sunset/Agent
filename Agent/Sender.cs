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
        public static string host = "";
        public static string baseUrl = "";

        private string name = "";

        private int cpu;
        private int mem;
        private int n = 0;

        private SysPerfomance sPerf;
        private Logger logger;

        public Sender(string name)
        {
            this.name = name;
            sPerf = new SysPerfomance();
            logger = new Logger(name);
            
        }

        public void Send()
        {
            n++;
            if (n == 10)
            {
                sPerf.GetPerformance(out cpu, out mem);
                n = 0;
            }
            
            //var param = Encoding.UTF8.GetBytes("?name=" + name + "&cpu=" + cpu + "&memory=" + mem);
            //url += Convert.ToBase64String(param);
            //Console.WriteLine(url);
            //var response = new HttpClient().GetAsync(url);
            var url = baseUrl + "?name=" + name + "&cpu=" + cpu + "&memory=" + mem;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Proxy = null;

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
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

        void WriteComment(object code)
        {
            if (Program.IsCommentEnabled)
            {
                Console.WriteLine(DateTime.Now + " " + code);
            }
        }

        void WriteLog(object code)
        {
            if (Program.IsLogEnabled)
            {
                logger.Write(code.ToString());
            }
        }

        public void CloseLog()
        {
            logger.EndLogging();
        }
    }
}
