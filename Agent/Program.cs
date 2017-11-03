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
        private static string host = "192.168.1.1";
        private static string name = "Server";
        public static bool isCommentEnabled = true;
        public static bool isLogEnabled = true;
        public static bool isCheckEnabled = true;
        private static int n = 10;

        private static Thread[] threads;
        private static Timer[] timers;
        private static Sender[] senders;
        private static Random random = new Random();
        private static List<string> names;

        static void Main(string[] args)
        {
            if (Init(args))
            {
                threads = new Thread[n];
                timers = new Timer[n];
                senders = new Sender[n];
                names = new List<string>(n);

                for (int i = 0; i < n; i++)
                {
                    while (names.Contains(name))
                    {
                        name = CreateName(5);
                    }
                    names.Add(name);

                    threads[i] = new Thread(RunMultiple);
                    threads[i].Start(i);
                }

                //names = null;

                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {

                }

                for (int i = 0; i < n; i++)
                {
                    threads[i].Join(500);
                    timers[i].Dispose();
                    senders[i].CloseLog();

                }

                Console.WriteLine("Exitting");
            }

        }

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

        static bool Init(string[] paramStrings)
        {

            for (int i = 0; i < paramStrings.Length; i++)
            {
                if (paramStrings[i] == "/n")
                    n = Convert.ToInt32(paramStrings[i + 1]);
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
                    if (ParseName(paramStrings[i + 1]))
                        name = paramStrings[i + 1];
                    else
                    {
                        Console.WriteLine("Cannot verify name");
                        return false;
                    }
                }
                if (paramStrings[i] == "/nolog")
                    isLogEnabled = false;
                if (paramStrings[i] == "/nocomment")
                    isCommentEnabled = false;
                if (paramStrings[i] == "/nocheck")
                    isCheckEnabled = false;

            }

            Sender.host = host;
            Sender.baseUrl = @"http://" + Sender.host + @"/SampleApi/api/performances/set/";
            Console.WriteLine("Running " + n + " agents to " + host);
            Console.WriteLine("Press Q to quit");
            return true;

        }

        static void RunMultiple(object data)
        {
            var i = (int)data;
            senders[i] = new Sender(names[i]);
            timers[i] = new Timer(Send, senders[i], 1000, 32);
        }

        static void Send(object data)
        {
            ((Sender)data).Send();
        }



        static string CreateName(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static bool ParseName(string s)
        {
            if (isCheckEnabled)
            {
                return Regex.IsMatch(s, @"^[\w]{1,10}$");
            }
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

    }
}
