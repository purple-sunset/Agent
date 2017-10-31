using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultipleInstance
{
    class Program
    {
        private static bool isCommentEnabled=true;
        private static bool isLogEnabled=true;
        private static string ip = "192.168.1.8";
        private static int n=10;

        private static AppDomain[] domains;
        private static Thread[] threads;
        private static Random random = new Random();

        static void Main(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == "/addr")
                    ip = args[i + 1];
                if (args[i] == "/n")
                    n = Convert.ToInt32(args[i + 1]);
                if (args[i] == "/nolog")
                    isLogEnabled = false;
                if (args[i] == "/nocomment")
                    isCommentEnabled = false;
                
            }
            threads = new Thread[n];
            domains = new AppDomain[n];

            for (int i = 0; i < n; i++)
            {
                domains[i] = AppDomain.CreateDomain("Agent " + i);
                threads[i] = new Thread(StartAgent);
                threads[i].Start(domains[i]);
            }

            Console.WriteLine("Press X to quit");
            while (Console.ReadKey(true).Key != ConsoleKey.X)
            {

            }
            
            for (int i = 0; i < n; i++) threads[i].Join();
            
            for (int i = 0; i < n; i++) AppDomain.Unload(domains[i]);
        }

        static void StartAgent(object domain)
        {
            ((AppDomain) domain).ExecuteAssembly("Agent.exe", new[] {"/addr", ip, "/name", CreateName(5), "/nocomment" });
        }

        static string CreateName(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
