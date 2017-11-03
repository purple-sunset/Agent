using System;
using System.Linq;
using System.Threading;

namespace Agent
{
    class Program
    {
        public static bool isCommentEnabled;
        public static bool isLogEnabled;
        public static bool isCheckEnabled = true;

        private static string host = "192.168.1.1";
        private static int n = 1;

        private static Thread[] threads;
        private static Timer[] timers;
        private static Sender[] senders;

        public static bool IsCommentEnabled
        {
            get => isCommentEnabled;
            set => isCommentEnabled = value;
        }

        public static bool IsLogEnabled
        {
            get => isLogEnabled;
            set => isLogEnabled = value;
        }

        private static void Main(string[] args)
        {
            if (Init(args))
            {
                threads = new Thread[n];
                timers = new Timer[n];
                senders = new Sender[n];

                for (var i = 0; i < n; i++)
                {
                    threads[i] = new Thread(RunMultiple);
                    threads[i].Start(i);
                }


                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {
                }

                Console.WriteLine("Exitting");

                for (var i = 0; i < n; i++)
                {
                    timers[i]?.Dispose();
                    senders[i].CloseLog(100);
                    threads[i].Join();
                }
            }
        }

        private static void RunMultiple(object data)
        {
            var i = (int) data;
            senders[i] = new Sender();
            if (senders[i].GetName())
                timers[i] = new Timer(Send, senders[i], 1000, 30);
        }

        private static void Send(object data)
        {
            ((Sender) data).Send();
        }

        private static bool Init(string[] paramStrings)
        {
            for (var i = 0; i < paramStrings.Length; i++)
            {
                if (paramStrings[i] == "/n")
                    n = Convert.ToInt32(paramStrings[i + 1]);
                if (paramStrings[i] == "/host")
                    if (ParseHost(paramStrings[i + 1]))
                    {
                        host = paramStrings[i + 1];
                    }
                    else
                    {
                        Console.WriteLine("Cannot verify host");
                        return false;
                    }

                if (paramStrings[i] == "/log")
                    isLogEnabled = true;
                if (paramStrings[i] == "/comment")
                    isCommentEnabled = true;
                if (paramStrings[i] == "/nocheck")
                    isCheckEnabled = false;
            }

            Sender.host = host;
            Sender.baseUrl = @"http://" + Sender.host + @"/SampleApi/api/performances/";
            Console.WriteLine("Init " + n + " agents to " + host);
            Console.WriteLine("Press Q to quit");
            return true;
        }

        private static bool ParseHost(string s)
        {
            if (isCheckEnabled)
            {
                var values = s.Split(':');
                var n = values.Length;
                if (n > 2)
                    return false;
                if (n > 0)
                {
                    var subvalues = values[0].Split('.');
                    if (subvalues.Length != 4)
                        return false;

                    if (n == 2)
                    {
                        int port;
                        if (!int.TryParse(values[1], out port))
                            return false;
                    }

                    byte tempForParsing;
                    return subvalues.All(r => byte.TryParse(r, out tempForParsing));
                }
                return false;
            }
            return true;
        }
    }
}