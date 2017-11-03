using System;
using System.IO;

namespace Agent
{
    class Logger
    {
        private readonly string fileLocation = "";
        private readonly FileStream fs;
        private readonly StreamWriter str;

        public Logger(string fileName)
        {
            fileLocation = Directory.GetCurrentDirectory() + "\\" + fileName + ".txt";
            fs = new FileStream(fileLocation, FileMode.OpenOrCreate);
            str = new StreamWriter(fs);
            str.BaseStream.Seek(0, SeekOrigin.End);
        }

        public void Write(string message)
        {
            str.WriteLine(DateTime.Now + " " + message);
            str.Flush();
        }

        public void EndLogging()
        {
            str.Close();
            fs.Close();
        }
    }
}