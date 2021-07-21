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
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string folder = "winget-new-repos";
        private static string fullPath = Path.Combine(directory, folder);
        private static string original = Path.Combine(fullPath, "original.txt");
        private static string middleware = Path.Combine(fullPath, "middleware.txt");
        private static string updated = Path.Combine(fullPath, "updated.txt");

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

                using (var myFile = File.CreateText(original))
                {
                    myFile.WriteLine(sb.ToString());
                }

            }
            catch (Exception objException)
            {
                Console.WriteLine(objException);
            }
        }

        private static void WriteToUpdated()
        {
            bool isNew = true;
            List<string> orig = File.ReadAllLines(original).ToList();
            List<string> mid = File.ReadAllLines(middleware).ToList();
            List<string> temp = new List<string>();
            List<string> updatedList;

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
                }
                isNew = true;
            }
            string marRes = string.Join(Environment.NewLine, temp.ToArray());
            if (temp.Count != 0)
            {
                File.WriteAllText(updated, marRes);
            }
            if (mid.Count == 0)
            {
                File.AppendAllText(middleware, marRes);
            }
            else
            {
                File.AppendAllText(middleware, Environment.NewLine + marRes);
            }


            updatedList = File.ReadAllLines(updated).ToList();

            for (int i = 0; i < updatedList.Count; i++)
            {
                Console.WriteLine(updatedList[i]);
            }
            Console.WriteLine("\nThere were {0} recently added or updated packages", updatedList.Count);
        }

        private static void WriteToOriginal()
        {
            Program.RunCommand(false);
        }

        private static void CreateFiles()
        {
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            CreateFile(original);
            CreateFile(middleware);
            CreateFile(updated);
        }


        private static void CreateFile(string filename)
        {
            if(!File.Exists(filename))
            {
                var file = File.Create(filename);
                file.Close();
            }
        }

    }
}