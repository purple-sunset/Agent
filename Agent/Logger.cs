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
        private string fileLocation = "";
        private FileStream fs;
        private StreamWriter str;

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
