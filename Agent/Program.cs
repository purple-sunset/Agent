using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Threading.Timer;



namespace Agent
{
    class Program
    {
        public static bool isCommentEnabled = false;
        public static bool isLogEnabled = false;
        public static bool isCheckEnabled = true;

        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 100000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SetTcpKeepAlive(true, 10000, 1000);
            ServicePointManager.UseNagleAlgorithm = false;
            if (Init(args))
            {
                Timer timer2 = new Timer(GetPer, null, 100, 40);
                Timer timer = new Timer(Send, null, 200, 40);

                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {

                }
                Console.WriteLine("Exitting");
                
                timer.Dispose();
                timer2.Dispose();
                if (isLogEnabled)
                {
                    Thread.Sleep(100);
                    Logger.EndLogging();
                }
            }

        }
        

        static bool Init(string[] paramStrings)
        {
            string host = "192.168.1.8";
            string name = "domain.com";
            for (int i = 0; i < paramStrings.Length; i++)
            {
                if (paramStrings[i] == "/host")
                {
                    if (ParseHost(paramStrings[i + 1]))
                        host = paramStrings[i + 1];
                    else
                    {
                        Console.WriteLine("Cannot verify host");
                        return false;
                    }
                }
                if (paramStrings[i] == "/name")
                {
                    name = paramStrings[i + 1];
                }
                if (paramStrings[i] == "/log")
                    isLogEnabled = true;
                if (paramStrings[i] == "/comment")
                    isCommentEnabled = true;
                if (paramStrings[i] == "/nocheck")
                    isCheckEnabled = false;

            }

            if (isCommentEnabled)
            {
                Console.WriteLine("Init agent to " + host);
                Console.WriteLine("Press Q to quit");
            }
            if(isLogEnabled)
                Logger.Init(name);
            Sender.Init(host, name);
            return true;

        }

        static bool ParseHost(string s)
        {
            if (isCheckEnabled)
            {
                var values = s.Split(':');
                int n = values.Length;
                if (n > 2)
                {
                    return false;
                }
                else if (n > 0)
                {
                    var subvalues = values[0].Split('.');
                    if (subvalues.Length != 4)
                    {
                        return false;
                    }

                    if (n == 2)
                    {
                        int port;
                        if (!Int32.TryParse(values[1], out port))
                            return false;
                    }

                    byte tempForParsing;
                    return subvalues.All(r => byte.TryParse(r, out tempForParsing));
                }
                return false;
            }
            return true;
        }

        static void Send(object sender)
        {
            Sender.Send();
        }

        static void GetPer(object sender)
        {
            SysPerfomance.GetPerformance(out Sender.cpu, out Sender.mem);
        }
    }
}
