using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using System.Windows.Forms;

namespace Thrashing_Detector
{
    class Monitors
    {
        // constants defining thrashing requirements
        const double PROC_THRASHING = 25;
        const double MEM_THRASHING = 80;
        const double HARDFAULTS_THRASHING = 1000;
        const int THRASHING_TIMER = 30;

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
        PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", "_Total");
        PerformanceCounter pagingCounter = new PerformanceCounter("Memory", "Page Reads/sec");
        //PerformanceCounter totalRam = new PerformanceCounter("Memory", "Total MBytes");

        // Poll the system for resource values
        public void PollFunction()
        {
            _procTime = cpuCounter.NextValue();
            _memoryUsed = (ramCounter.NextValue() / _totalMemory) * 100;
            _pageFaults = pagingCounter.NextValue();
            _records.Add(new Tuple<double, double, double>(_procTime, _memoryUsed, _pageFaults));
            ThrashingCheck();
        }

        internal void ThrashingCheck()
        {
            if (_memoryUsed >= MEM_THRASHING && _procTime <= PROC_THRASHING && _pageFaults >= HARDFAULTS_THRASHING)
            {
                _thrashingCounter += 1;
            }
            else if (_thrashingCounter > 0)
            {
                _thrashingCounter -= 1;
            }

            if(_thrashingCounter >= THRASHING_TIMER)
            {
                MessageBox.Show("Thrashing is occuring!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                _thrashingCounter = 0;
            }

        }

        // constructor
        internal Monitors()
        {
            _procTime = 0;
            _memoryUsed = 0;
            _totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            _pageFaults = 0;
            _thrashingCounter = 0;
            _records = new List<Tuple<double, double, double>>();
        }
    }
}
