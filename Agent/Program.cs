using System;
using System.Collections.Generic;
using System.Linq;
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
        public static bool isLogEnabled = true;
        public static bool isCheckEnabled = true;

        static void Main(string[] args)
        {
            if (Init(args))
            {
                Timer timer = new Timer(Send, null, 1000, 32);

                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {

                }
                Console.WriteLine("Exitting");
                timer.Dispose();
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

            return Sender.Init(host);

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
            Sender.Send2();
        }

    }
}
