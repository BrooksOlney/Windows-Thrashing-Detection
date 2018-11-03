using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Thrashing_Detector
{
    class Program
    {
        static void Main(string[] args)
        {
            Monitors _monitors = new Monitors();
            StreamWriter sw = new StreamWriter("resource_stats.csv");
            while (!Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(125);
                _monitors.PollFunction();
                _monitors.ThrashingCheck();
                _monitors.processes();
                ClearConsole();
                Console.WriteLine("CPU Time: {0}%\nMemory Used: {1}%\nHard Page Faults/sec: {2}Thrashing counter: {3}",
                    _monitors._procTime.ToString("N2"), _monitors._memoryUsed.ToString("N2"), _monitors._pageFaults, _monitors._thrashingCounter);
                sw.WriteLine("{0}, {1}, {2}, {3}", DateTime.Now.TimeOfDay, _monitors._memoryUsed.ToString("N2"), _monitors._procTime.ToString("N2"), _monitors._pageFaults);
            }

            sw.Close();
            Console.WriteLine("Testing...");
        }

        static void ClearConsole()
        {
            Console.Clear();
            Console.WriteLine("Press 'c' to terminate current operations.");
        }
    }
}
