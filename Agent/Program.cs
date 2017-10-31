using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Threading.Timer;



namespace Agent
{
    class Program
    {
        private static bool isCommentEnabled = true;
        private static bool isLogEnabled = true;
        private static string ip = "";
        private static string name = "";
        private static int cpu;
        private static int mem;

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "/addr")
                    ip = args[i + 1];
                if (args[i] == "/name")
                    name = args[i + 1];
                if (args[i] == "/nolog")
                    isLogEnabled = false;
                if (args[i] == "/nocomment")
                    isCommentEnabled = false;
            }
            Console.WriteLine("Press Q to quit");
            Timer timer = new Timer(GetPerformance,null,1000,40);

            while (Console.ReadKey(true).Key != ConsoleKey.Q)
            {

            }

            timer.Dispose();
            Console.WriteLine("Test");

        }


        static void GetPerformance(object sender)
        {
            SysPerfomance.GetPerformance(out cpu, out mem);

        }
        
    }
}
