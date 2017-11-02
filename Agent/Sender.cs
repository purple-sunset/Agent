﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Agent
{
    class Sender
    {
        private static string host = "";
        private static string name = "";
        private static string baseUrl = "";

        public static void Init(string host, string name)
        {
            Sender.host = host;
            Sender.name = name;
            baseUrl = @"http://" + Sender.host + @"/SampleApi/api/performances/set/";
        }

        public static void Send()
        {
            
            //var param = Encoding.UTF8.GetBytes("?name=" + name + "&cpu=" + cpu + "&memory=" + mem);
            //url += Convert.ToBase64String(param);
            //Console.WriteLine(url);
            //var response = new HttpClient().GetAsync(url);
            var url = baseUrl + "?name=" + name + "&cpu=" + SysPerfomance.cpu + "&memory=" + SysPerfomance.mem;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Proxy = null;

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var code = response.StatusCode;
                    WriteComment(" cpu= " + SysPerfomance.cpu + " memory= " + SysPerfomance.mem + " " + code);
                    WriteLog(" cpu= " + SysPerfomance.cpu + " memory= " + SysPerfomance.mem + " " + code);
                    /*if (code != HttpStatusCode.OK)
                    {
                        
                    }*/
                }
                    
                
            }
            catch (Exception e)
            {
                WriteComment(e.Message);
                WriteLog(e.Message);
            }
        }

        static void WriteComment(object code)
        {
            if (Program.IsCommentEnabled)
            {
                Console.WriteLine(DateTime.Now + " " + code);
            }
        }

        static void WriteLog(object code)
        {
            if (Program.IsLogEnabled)
            {
                Logger.Write(code.ToString());
            }
        }
    }
}
