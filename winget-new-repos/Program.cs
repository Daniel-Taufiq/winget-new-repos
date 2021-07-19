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
            Program.CreateFiles();
            Program.WriteToOriginal();
            Program.WriteToUpdated();
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }

        public static void RunCommand(bool readOutput)
        {
            string id = "Name";
            bool startCopying = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + "winget search");

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    // ignore meta data at the start
                    if (startCopying == true || line.Split(' ')[0] == id)
                    {
                        startCopying = true;
                        sb.Append(line + Environment.NewLine);
                    }
                }
                File.WriteAllText("original.txt", sb.ToString());
            }
            catch (Exception objException)
            {
                Console.WriteLine(objException);
            }
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


            if(updated.Count == 0)
            {
                Console.WriteLine("Name                             Id                                 Version");
                Console.WriteLine("------------------------------------------------------------------------------------------");
            }

            for(int i = 0; i < updated.Count; i++)
            {
                Console.WriteLine(updated[i]);
            }
            Console.WriteLine("\nThere were {0} recently added or updated packages", counter);
        }

        private static void WriteToOriginal()
        {
            Program.RunCommand(false);
        }

        private static void CreateFiles()
        {
            if (!File.Exists("original.txt"))
            {
                CreateFile("original.txt");
            }
            if (!File.Exists("middleware.txt"))
            {
                CreateFile("middleware.txt");
            }
            if (!File.Exists("updated.txt"))
            {
                CreateFile("updated.txt");
            }
        }

        private static void CreateFile(string filename)
        {
            var file = File.Create(filename);
            file.Close();
        }
        
    }
}
