using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;

namespace Thrashing_Detector
{
    class Monitors
    {
        Processes _procs { get; set; }
        Memory _memory { get; set; }
        VirtualMemory _vm { get; set; }

        public double _procTime { get; set; }
        public double _memoryUsed { get; set; }
        public double _pageFaults { get; set; }

        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        PerformanceCounter pagingCounter = new PerformanceCounter("Processor", "Page Faults/sec");

        public void PollFunction()
        {
            _procTime = cpuCounter.NextValue();
            _memoryUsed = ramCounter.NextValue();
            _pageFaults = pagingCounter.NextValue();
        }

        internal Monitors()
        {
            // system resource objects
            _procs = new Processes();
            _memory = new Memory();
            _vm = new VirtualMemory();
            _procTime = 0;
            _memoryUsed = 0;
            _pageFaults = 0;
        }

        private void GetTotals()
        {

        }

        class Processes
        {
            List<Process> _procs = new List<Process>();

            internal Processes()
            {
                _procs = Process.GetProcesses().ToList();
            }
        }

        class Memory
        {

        }

        class VirtualMemory
        {

        }
    }
}
