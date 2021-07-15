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
            //Program.WriteToOriginal();
            Program.WriteToUpdated();
        }

        public static string RunCommand(bool readOutput)
        {
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
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                //Console.WriteLine(result);
                return result;
            }
            catch (Exception objException)
            {
                Console.WriteLine(objException);
                return null;
            }
        }

        private static void WriteToUpdated()
        {
            int counter = 0;
            string line;
            ArrayList updatedList = new ArrayList();
            bool isNew = true;
            List<string> allLines = File.ReadAllLines("original.txt").ToList();
            List<string> allLinesUpdated = File.ReadAllLines("updated.txt").ToList();
            // need 3rd list to add

            for (int i = 0; i < allLines.Count; i++)
            {
                for (int j = 0; j < allLinesUpdated.Count; j++)
                {
                    if (allLines[i] == allLinesUpdated[j])
                    {
                        isNew = false;
                    }
                }
                if (isNew == true)
                {
                    allLinesUpdated.Add(allLines[i]);
                    //Console.WriteLine(allLines[i]);
                }
                isNew = true;
            }
            for(int i = 0; i < allLinesUpdated.Count; i++)
            {
                Console.WriteLine(allLinesUpdated[i]);
            }
            string marRes = string.Join(Environment.NewLine, allLinesUpdated.ToArray());
            //Console.WriteLine("marRes is: " + marRes);
            File.WriteAllText("updated.txt", marRes);
        }

        private static void WriteToOriginal()
        {
            string output = Program.RunCommand(false);
            File.WriteAllText("original.txt", output);
        }
        
    }
}
