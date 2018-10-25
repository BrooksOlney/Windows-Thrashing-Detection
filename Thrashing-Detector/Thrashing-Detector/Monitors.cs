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
        // Member variables for different resource types 
        public double _procTime { get; set; }
        public double _memoryUsed { get; set; }
        public float  _totalMemory { get; set; }
        public double _pageFaults { get; set; }
        public int    _thrashingCounter { get; set; }

        // Records all results in real time
        public List<Tuple<double, double, double>> _records { get; set; }

        // Performance counters for different resource types
        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set - Private", "_Total");
        PerformanceCounter pagingCounter = new PerformanceCounter("Memory", "Page Reads/sec");
        //PerformanceCounter totalRam = new PerformanceCounter("Memory", "Total MBytes");

        // Poll the system for resource values
        public void PollFunction()
        {
            _procTime = cpuCounter.NextValue();
            _memoryUsed = ramCounter.NextValue() / 1048576;
            _pageFaults = pagingCounter.NextValue();
            _records.Add(new Tuple<double, double, double>(_procTime, _memoryUsed, _pageFaults));
        }

        // constructor
        internal Monitors()
        {
            _procTime = 0;
            _memoryUsed = 0;
           // _totalMemory = totalRam.NextValue();
            _pageFaults = 0;
            _records = new List<Tuple<double, double, double>>();
        }
    }
}
