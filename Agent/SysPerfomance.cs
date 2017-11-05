using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Management;

namespace Agent
{
    class SysPerfomance
    {
        private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        private static PerformanceCounter memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        

        //private static int coreCount = Environment.ProcessorCount;
        //private static ManagementObjectSearcher searcher = new ManagementObjectSearcher("select PercentProcessorTime from Win32_PerfFormattedData_PerfOS_Processor where Name=\"_Total\"");


        public static void GetPerformance(out int cpu, out int mem)
        {
            cpu = (int) cpuCounter.NextValue();
            mem = (int) memCounter.NextValue();
            //Console.WriteLine(cpu + "   " + mem);
        }

        
        /*public static void GetPerformance2(out int cpu, out int mem)
        {
            cpu = 0;
            foreach (ManagementObject obj in searcher.Get())
            {
                cpu = Convert.ToInt32(obj["PercentProcessorTime"]);
                
            }
            
            mem = 20;
            Console.WriteLine(cpu + "   " + mem);
        }*/
    }
}
