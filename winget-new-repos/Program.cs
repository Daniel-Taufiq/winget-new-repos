using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace winget_new_repos
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.WriteToOriginal();
            Program.WriteToUpdated();
        }

        public static void RunCommand(bool readOutput)
        {
            string id = "Name                             Id                                 Version";
            var processInfo = new ProcessStartInfo("cmd.exe", "/c winget search")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = @"C:\Windows\System32\"
            };

            StringBuilder sb = new StringBuilder();
            Process p = Process.Start(processInfo);
            bool startCopying = false;
            while (!p.StandardOutput.EndOfStream)
            {
                var line = p.StandardOutput.ReadLine();
                if (startCopying == true || line == id)
                {
                    startCopying = true;
                    sb.Append(line + Environment.NewLine);
                }
                Console.WriteLine(line);
            }
            File.WriteAllText("original.txt", sb.ToString());

        }

        private static void WriteToUpdated()
        {
            int counter = 0;
            bool isNew = true;
            List<string> orig = File.ReadAllLines("original.txt").ToList();
            List<string> mid = File.ReadAllLines("middleware.txt").ToList();

            List<string> temp = new List<string>();

            for (int i = 0; i < orig.Count; i++)
            {
                for (int j = 0; j < mid.Count; j++)
                {
                    if (orig[i] == mid[j])
                    {
                        isNew = false;
                    }
                }
                if (isNew == true)
                {
                    //Console.WriteLine("added repo: " + orig[i]);
                    temp.Add(orig[i]);
                    counter++;
                }
                isNew = true;
            }
            string marRes = string.Join(Environment.NewLine, temp.ToArray());
            if(temp.Count != 0)
            {
                File.WriteAllText("updated.txt", marRes);
            }
            if(mid.Count == 0)
            {
                File.AppendAllText("middleware.txt", marRes);
            }
            else
            {
                File.AppendAllText("middleware.txt", Environment.NewLine + marRes);
            }


            List<string> updated = File.ReadAllLines("updated.txt").ToList();

            for(int i = 0; i < updated.Count; i++)
            {
                Console.WriteLine(updated[i]);
            }
            Console.WriteLine("There were {0} recently added packages", counter);
        }

        private static void WriteToOriginal()
        {
            Program.RunCommand(false);
            //File.WriteAllText("original.txt", output);
        }
        
    }
}
