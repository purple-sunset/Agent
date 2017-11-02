using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MultipleInstance
{
    class Program
    {


        private static int n = 10;
        private static string host = "192.168.1.1";
        private static bool isLogEnabled = true;

        private static AppDomain[] domains;
        private static Thread[] threads;
        private static Random random = new Random();
        private static List<string> names;

        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        static void Main(string[] args)
        {
            if (Init(args))
            {
                threads = new Thread[n];
                domains = new AppDomain[n];
                names = new List<string>(n);
                string name = "";

                for (int i = 0; i < n; i++)
                {
                    do
                    {
                        name = CreateName(5);
                    } while (names.Contains(name));
                    names.Add(name);
                    domains[i] = AppDomain.CreateDomain("Agent " + i);
                    threads[i] = new Thread(StartAgent);
                    threads[i].Start(domains[i]);
                }


                while (Console.ReadKey(true).Key != ConsoleKey.X)
                {

                }

                for (int i = 0; i < n; i++) threads[i].Join();

                for (int i = 0; i < n; i++) AppDomain.Unload(domains[i]);
            }

        }

        static void StartAgent(object domain)
        {
            var result = Regex.Match(((AppDomain)domain).FriendlyName, @"\d+").Value;
            var t = Int32.Parse(result);
            var a = names[t];
            if (isLogEnabled)
                ((AppDomain)domain).ExecuteAssembly("Agent.exe", new[] { "/nocomment", "/nocheck", "/host", host, "/name", a });
            else
            {
                ((AppDomain)domain).ExecuteAssembly("Agent.exe", new[] { "/nocomment", "/nolog", "/nocheck", "/host", host, "/name", a });
            }
        }



        static string CreateName(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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

                if (paramStrings[i] == "/nolog")
                    isLogEnabled = false;

            }

            Console.WriteLine("Running " + n + " agents to " + host);
            return true;
        }


        static bool ParseHost(string s)
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
    }
}
