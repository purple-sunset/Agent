using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Agent
{
    class Sender
    {
        public static string host = "";
        public static string baseUrl = "";

        private static readonly char[] charsToTrim = {'"', '\\'};

        private int cpu;
        private int mem;
        private int n;
        private string name = "";

        private Logger logger;
        private SysPerfomance sPerf;


        public bool GetName()
        {
            var request = (HttpWebRequest) WebRequest.Create(baseUrl + "getname");
            request.KeepAlive = false;
            request.Proxy = null;
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var code = response.StatusCode;
                    if (code == HttpStatusCode.OK)
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            var tname = reader.ReadToEnd().Trim(charsToTrim);
                            if (tname.Length > 0)
                            {
                                name = tname;
                                sPerf = new SysPerfomance();
                                if (Program.isLogEnabled)
                                    logger = new Logger(name);
                                return true;
                            }
                        }
                }
            }
            catch (Exception e)
            {
                WriteComment(e.Message);
                WriteLog(e.Message);
            }

            return false;
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
            var url = baseUrl + "set/?name=" + name + "&cpu=" + cpu + "&memory=" + mem;
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.KeepAlive = false;
            request.Proxy = null;

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var code = response.StatusCode;
                    WriteComment(" cpu= " + cpu + " memory= " + mem + " " + code);
                    WriteLog(" cpu= " + cpu + " memory= " + mem + " " + code);
                    
                }
            }
            catch (Exception e)
            {
                WriteComment(e.Message);
                WriteLog(e.Message);
            }
        }

        private void WriteComment(object code)
        {
            if (Program.IsCommentEnabled)
                Console.WriteLine(DateTime.Now + " " + code);
        }

        private void WriteLog(object code)
        {
            if (Program.IsLogEnabled)
                logger.Write(code.ToString());
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