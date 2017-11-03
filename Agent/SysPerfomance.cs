using System.Diagnostics;

namespace Agent
{
    class SysPerfomance
    {
        private readonly PerformanceCounter cpuCounter =
            new PerformanceCounter("Processor Information", "% Processor Time", "_Total");

        private readonly PerformanceCounter memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");

        //private static int coreCount = Environment.ProcessorCount;
        //private static ManagementObjectSearcher searcher = new ManagementObjectSearcher("select PercentProcessorTime from Win32_PerfFormattedData_PerfOS_Processor where Name=\"_Total\"");

        public void GetPerformance(out int cpu, out int mem)
        {
            cpu = (int) cpuCounter.NextValue();
            mem = (int) memCounter.NextValue();
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