using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Thrashing_Detector
{
    class Monitors
    {
        Processes _procs { get; set; }
        Memory _memory { get; set; }
        VirtualMemory _vm { get; set; }



        internal Monitors()
        {
            // system resource objects
            _procs = new Processes();
            _memory = new Memory();
            _vm = new VirtualMemory();
        }

        class Processes
        {
            List<Process> _procs = new List<Process>()
                .Where(p => p.);

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


        public void PollFunction()
        {

        }
    }
}
