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
        //Processes _procs { get; set; }
        //Memory _memory { get; set; }
        //VirtualMemory _vm { get; set; }

        public double _procTime { get; set; }
        public double _memoryUsed { get; set; }
        public double _pageFaults { get; set; }
        public List<Tuple<double, double, double>> _records { get; set; }

        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        PerformanceCounter pagingCounter = new PerformanceCounter("Memory", "Page Faults/sec");

        public void PollFunction()
        {
            _procTime = cpuCounter.NextValue();
            _memoryUsed = ramCounter.NextValue();
            _pageFaults = pagingCounter.NextValue();
            _records.Add(new Tuple<double, double, double>(_procTime, _memoryUsed, _pageFaults));
        }

        internal Monitors()
        {
            _procTime = 0;
            _memoryUsed = 0;
            _pageFaults = 0;
            _records = new List<Tuple<double, double, double>>();
        }
    }
}
