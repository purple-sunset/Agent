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
        public static bool isCommentEnabled = true;
        public static bool isLogEnabled = true;
        public static bool isCheckEnabled = true;

        static void Main(string[] args)
        {
            if (Init(args))
            {
                Timer pTimer = new Timer(GetPerformance, null, 500, 500);
                Timer sTimer = new Timer(Send, null, 1000, 32);

                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {

                }

                sTimer.Dispose();
                pTimer.Dispose();
                if (isLogEnabled)
                    Logger.EndLogging();
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
            string host = "192.168.1.1";
            string name = "Server";
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

            Sender.Init(host, name);
            if (isLogEnabled)
                Logger.Init(name);
            if (IsCommentEnabled)
            {
                Console.WriteLine("Sending data to " + host);
                Console.WriteLine("Press Q to quit");
            }
            return true;

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

        static void Send(object sender)
        {
            Sender.Send();
        }

        static void GetPerformance(object data)
        {
            SysPerfomance.GetPerformance();
        }
    }
}
