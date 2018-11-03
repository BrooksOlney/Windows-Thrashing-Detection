using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using System.Windows.Forms;
using System.IO;

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
        PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available Bytes");
        PerformanceCounter pagingCounter = new PerformanceCounter("Memory", "Page Reads/sec");
        //PerformanceCounter totalRam = new PerformanceCounter("Memory", "Total MBytes");

        // Poll the system for resource values
        public void PollFunction()
        {
            _procTime = cpuCounter.NextValue();
            _memoryUsed = ((_totalMemory - ramCounter.NextValue()) / _totalMemory) * 100;
            _pageFaults = pagingCounter.NextValue();
            _records.Add(new Tuple<double, double, double>(_procTime, _memoryUsed, _pageFaults));
            ThrashingCheck();
        }

        internal void ThrashingCheck()
        {
            if (_memoryUsed > MEM_THRASHING)
            {
                if (_pageFaults > HARDFAULTS_THRASHING)
                {
                    _thrashingCounter++;
                    if (_thrashingCounter == THRASHING_TIMER / 2)
                    {
                        processes();
                    }

                    if (_thrashingCounter == THRASHING_TIMER)
                    {
                        MessageBox.Show("Thrashing is occuring!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        _thrashingCounter = 0;
                    }
                }
                else
                {
                    _thrashingCounter--;
                }

            }
        }
        //function to capture the processes
        internal void processes()
        {
            List<Process> proclist = Process.GetProcesses().ToList();
            proclist = proclist.OrderByDescending(o => o.PeakWorkingSet64).ToList();
            StreamWriter sw = new StreamWriter("process_stats.csv");

            for (int i = 0; i < 10; i++)
            {
                sw.WriteLine("Process: {0} ID: {1} Memory: {2}", proclist[i].ProcessName, proclist[i].Id, proclist[i].PeakWorkingSet64);
            }
            sw.Close();
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
