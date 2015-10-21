using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace DeveloperNewsWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var Tw = new Thread(new ThreadStart(Watcher));
            Tw.Start();
        }
        static void Watcher()
        {
            while(true)
            {
                var arr = Process.GetProcessesByName("DeveloperNews2");
                if (arr.Length < 1)
                {
                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "DeveloperNews2.exe";
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                Thread.Sleep(6000);
            }
        }
    }
}
