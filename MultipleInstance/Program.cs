using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleInstance
{
    class Program
    {
        private static bool isCommentEnabled=true;
        private static bool isLogEnabled=true;
        private static string ip = "";

        static void Main(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == "/addr")
                    ip = args[i + 1];
                if (args[i] == "/nolog")
                    isLogEnabled = false;
                if (args[i] == "/nocomment")
                    isCommentEnabled = false;
            }
            
        }

        
    }
}
