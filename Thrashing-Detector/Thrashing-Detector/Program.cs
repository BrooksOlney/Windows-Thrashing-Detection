using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Pipes;

namespace Thrashing_Detector
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter datacollection = new StreamWriter("training_set.csv");

            if (!File.Exists("settings.config"))
            {
                MessageBox.Show("Include configuration in 'settings.config' file. Program is exiting.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            double procthrashing = 0, memthrashing = 0, hardfaultsthrashing = 0;
            int thrashingTimer = 0;
            bool dataCollection = false;

            var readLines = File.ReadLines("settings.config");
            foreach(string line in readLines)
            {
                var setting = line.Split('=');
                if(setting.Length == 2)
                {
                    switch (setting[0])
                    {
                        case "procthreshold":
                            Double.TryParse(setting[1], out procthrashing);
                            break;
                        case "memthreshold":
                            Double.TryParse(setting[1], out memthrashing);
                            break;
                        case "hardfaultsthreshold":
                            Double.TryParse(setting[1], out hardfaultsthrashing);
                            break;
                        case "thrashingtimer":
                            int.TryParse(setting[1], out thrashingTimer);
                            break;
                        case "datacollection":
                            dataCollection = setting[1] == "true" ? true : false;
                            break;
                        default:
                            break;

                    }
                }
            }

            Monitors _monitors = new Monitors(procthrashing, memthrashing, hardfaultsthrashing, thrashingTimer);

            Console.WriteLine("Opening named-pipe for interprocess communication...");
            NamedPipeServerStream npss = new NamedPipeServerStream("pytorchModel", PipeDirection.InOut, 1, PipeTransmissionMode.Message);

            Console.WriteLine("Waiting for inbound connection from machine learning model...");
            npss.WaitForConnection();

            ClearConsole();
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            npss.Write(ms.ToArray(), 0, ms.ToArray().Length);
            ms.SetLength(0);
            
            while (!Console.KeyAvailable && npss.IsConnected)
            {
                using(FileStream fileHandle = new FileStream("torch/data/testing_set.csv", FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter sw = new StreamWriter(fileHandle))
                    {
                        int count = 0;
                        while(count++ < 10000 && !Console.KeyAvailable)
                        {
                            System.Threading.Thread.Sleep(125);
                            _monitors.PollFunction();
                            _monitors.ThrashingCheck();

                            if (dataCollection)
                                datacollection.WriteLine("{0}, {1}, {2}, {3}", _monitors._procTime.ToString("N2"), _monitors._memoryUsed.ToString("N2"), _monitors._pageFaults.ToString("N2"), _monitors._thrashingOccurance == true ? "1" : "0");

                            ClearConsole();
                            Console.WriteLine("CPU Time: {0}%\nMemory Used: {1}%\nHard Page Faults/sec: {2}\nThrashing counter: {3}",
                                _monitors._procTime.ToString("N2"), _monitors._memoryUsed.ToString("N2"), _monitors._pageFaults, _monitors._thrashingCounter);

                            try
                            {
                                bw.Write(String.Format(" {0}, {1}, {2} ", _monitors._procTime.ToString("N2"), _monitors._memoryUsed.ToString("N2"), _monitors._pageFaults.ToString("N2")));
                                npss.WriteAsync(ms.ToArray(), 0, ms.ToArray().Length);
                                ms.SetLength(0);
                            } catch(Exception ex)
                            {
                                Console.WriteLine("PyTorch has disconnected.");
                                break;
                            }

                            sw.WriteLine("{0}, {1}, {2}", _monitors._procTime.ToString("N2"), _monitors._memoryUsed.ToString("N2"), _monitors._pageFaults.ToString("N2"));
                        }
                    }
                }
            }

            datacollection.Close();
            npss.Close();
            ms.Close();
            bw.Close();
            Console.WriteLine("Closing...");
            Console.ReadKey();
        }

        static void ClearConsole()
        {
            Console.Clear();
            Console.WriteLine("Press 'c' to terminate current operations.");
        }
    }
}
