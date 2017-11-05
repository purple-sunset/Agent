using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Agent
{
    class Sender
    {
        public static string baseUrl = "";

        private string name = "";
        private Logger logger;
        
        //private static HttpClient client = new HttpClient();
        

        public Sender(string name)
        {
            this.name = name;
            if(Program.isLogEnabled)
                logger = new Logger(name);
            
        }
        

        public void Send()
        {

            var url = baseUrl + "?cpu=" + SysPerfomance.cpu + "&memory=" + SysPerfomance.mem + "&hostname=" + name;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = true;
            request.Proxy = null;

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


        void WriteComment(object code)
        {
            if (Program.isCommentEnabled)
            {
                Console.WriteLine(DateTime.Now + " " + code);
            }
        }

        void WriteLog(object code)
        {
            if (Program.isLogEnabled)
            {
                logger.Write(code.ToString());
            }
        }

        public void CloseLog(int delay)
        {
            if (Program.isLogEnabled)
            {
                Thread.Sleep(delay);
                logger?.EndLogging();
            }
        }
    }
}
