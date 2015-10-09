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
            var arr = Process.GetProcessesByName("DeveloperNews");
            if(arr.Length < 1)
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "DeveloperNews.exe";
                    process.Start();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            Thread.Sleep(6000);
            Watcher();
        }
    }
}
