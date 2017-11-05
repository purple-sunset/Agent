using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    class Logger
    {
        private static string fileLocation = "";
        private static FileStream fs;
        private static StreamWriter str;

        public static void Init(string fileName)
        {
            fileLocation = Directory.GetCurrentDirectory() + "\\" + fileName + ".txt";
            fs = new FileStream(fileLocation, FileMode.OpenOrCreate);
            str = new StreamWriter(fs);
            str.BaseStream.Seek(0, SeekOrigin.End);
        }

        public static void Write(string message)
        {
            str.WriteLine(DateTime.Now + " " + message);
            str.Flush();
        }

        public static void EndLogging()
        {
            str.Close();
            fs.Close();
        }
    }
}
