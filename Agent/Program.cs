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
        public static bool isCommentEnabled = true;
        public static bool isLogEnabled = true;
        
        static void Main(string[] args)
        {
            string ip = "192.168.1.8";
            string name = "a";
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

            Sender.Init(ip,name);
            if(isLogEnabled)
                Logging.Init(name);
            if(IsCommentEnabled)
                Console.WriteLine("Press Q to quit");

            Timer timer = new Timer(Send,null,1000,35);
            

            while (Console.ReadKey(true).Key != ConsoleKey.Q)
            {

            }

            timer.Dispose();
            if (isLogEnabled)
                Logging.EndLogging();
            Console.WriteLine("Test");

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

        static void Send(object sender)
        {
            Sender.Send();
        }
        
    }
}
