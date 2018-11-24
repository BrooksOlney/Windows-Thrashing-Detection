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
        public double PROC_THRASHING { get; set; }
        public double MEM_THRASHING { get; set; }
        public double HARDFAULTS_THRASHING { get; set; }
        public int THRASHING_TIMER { get; set; }

        // Member variables for different resource types 
        public double _procTime { get; set; }
        public double _memoryUsed { get; set; }
        public float  _totalMemory { get; set; }
        public double _pageFaults { get; set; }
        public int    _thrashingCounter { get; set; }
        public bool   _thrashingOccurance { get; set; }

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
                    _thrashingOccurance = true;
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
                else if(_thrashingCounter > 0)
                {
                    _thrashingOccurance = false;
                    _thrashingCounter--;
                }

            }
            else
            {
                _thrashingOccurance = false;
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
        internal Monitors(double procthrashing = 25, double memthrashing = 85, double hardfaultsthrashing = 300, int thrashingtimer = 30)
        {
            PROC_THRASHING = procthrashing;
            MEM_THRASHING = memthrashing;
            HARDFAULTS_THRASHING = hardfaultsthrashing;
            THRASHING_TIMER = thrashingtimer;

            _procTime = 0;
            _memoryUsed = 0;
            _totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            _pageFaults = 0;
            _thrashingCounter = 0;
            _thrashingOccurance = false;
            _records = new List<Tuple<double, double, double>>();
        }
    }
}
