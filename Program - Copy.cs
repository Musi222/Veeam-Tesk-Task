using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static void Main (string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: ProcessMonitor.exe <process:name> <maxLifetimeInMinutes> <monitoringFrequencyInMinutes>");
            return;
        }

        string processName = args[0];
        int maxLifetimeInMinutes = int.Parse(args[1]);
        int monitoringFrequencyInMinutes = int.Parse(args[2]);

        while (true)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                // Wrong name or not running
                Console.WriteLine($"No process with name '{processName}' is currently running.");
            }
            else
            {

                foreach (Process process in processes)
                {
                    // Check the time when you add it 
                    TimeSpan lifetime = DateTime.Now - process.StartTime;

                    if (lifetime.TotalMinutes > maxLifetimeInMinutes)
                    {
                        Console.WriteLine($"Killing process {process.Id} ({process.ProcessName}) that has been running for {lifetime.TotalMinutes} minutes.");

                        using (StreamWriter logFile = new StreamWriter("process_monitor.log", true))
                        {
                            logFile.WriteLine($"Killed process {process.Id} ({process.ProcessName}) that had been running for {lifetime.TotalMinutes} minutes.");
                        }
                        // Kill the proccess after the time
                        process.Kill();
                    }
                }
            }

            Thread.Sleep(monitoringFrequencyInMinutes * 60 * 1000); // Sleep for the specified monitoring frequency
        }
    }
}
