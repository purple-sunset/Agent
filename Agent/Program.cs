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
        private static int n = 10;
        private static int startNumber = 1;

        public static bool isCommentEnabled = false;
        public static bool isLogEnabled = false;
        public static bool isCheckEnabled = true;

        private static Thread[] threads;
        private static Timer[] timers;
        private static Sender[] senders;
        private static string[] names;

        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 100000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SetTcpKeepAlive(true, 10000, 1000);
            ServicePointManager.UseNagleAlgorithm = false;
            if (Init(args))
            {
                int due = 10;
                if (n < 20)
                    due = 41 - n;

                Timer timer = new Timer(GetPer, null, 100, due);

                threads = new Thread[n];
                timers = new Timer[n];
                senders = new Sender[n];
                for (int i = 0; i < n; i++)
                {
                    threads[i] = new Thread(RunMultiple);
                    threads[i].Start(i);
                }



                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {

                }
                Console.WriteLine("Exitting");

                timer.Dispose();
                for (var i = 0; i < n; i++)
                {
                    timers[i]?.Dispose();
                    senders[i].CloseLog(100);
                    threads[i].Join();
                }

            }

        }


        static bool Init(string[] paramStrings)
        {
            string host = "192.168.1.1:8080";
            string name = "domain1.com";
            for (int i = 0; i < paramStrings.Length; i++)
            {
                if (paramStrings[i] == "/n")
                    n = Convert.ToInt32(paramStrings[i + 1]);
                if (paramStrings[i] == "/s")
                    startNumber = Convert.ToInt32(paramStrings[i + 1]);
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


            Console.WriteLine("Init " + n + " agent to " + host);
            Console.WriteLine("Press Q to quit");

            names = new string[n];
            names[0] = name;
            for (int i = 1; i < n; i++)
            {
                names[i] = "domain" + (startNumber + i) + ".com";
            }
            
            Sender.baseUrl = @"http://" + host + @"/api/v1";
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


        private static void RunMultiple(object data)
        {
            var i = (int)data;
            senders[i] = new Sender(names[i]);
            timers[i] = new Timer(Send, senders[i], 1000, 30);
        }

        private static void Send(object data)
        {
            ((Sender)data).Send();
        }

        static void GetPer(object sender)
        {
            SysPerfomance.GetPerformance();
        }
    }
}
