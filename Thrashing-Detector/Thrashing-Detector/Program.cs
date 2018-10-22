using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Thrashing_Detector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing...");

            //List<Process> _procs = new List<Process>(System.Diagnostics.Process.GetProcesses());
            Monitors _monitors = new Monitors();
            while (true)
            {
                System.Threading.Thread.Sleep(250);
                _monitors.PollFunction();
                Console.Clear();
                Console.WriteLine("CPU Time: {0}%\nMemory Used: {1}MB\nPage Faults/sec: {2}",
                    _monitors._procTime.ToString("N2"), _monitors._memoryUsed, _monitors._pageFaults);
            }

            Console.WriteLine("Testing...");
        }
    }
}
