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
            _monitors.PollFunction();

            Console.WriteLine("Testing...");
        }
    }
}
