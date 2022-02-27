using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ApplicationForTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            cpDirCommand(@"C:\Program Files (x86)\Fallout 4", @"C:\Program Files (x86)\Fallout4New");
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        static void cpDirCommand(string SourcePath, string DestinationPath)
        {
            string[] DirAndFiles = Directory.EnumerateFileSystemEntries(SourcePath).ToArray();

            for (int i = 0; i < DirAndFiles.Length; i++)
            {
                try
                {
                    string NewfilePathInDir = @$"{DestinationPath}\{Path.GetFileName(DirAndFiles[i])}";
                    if (File.Exists(NewfilePathInDir))
                    {
                        continue;
                    }
                    File.Copy(DirAndFiles[i], NewfilePathInDir);
                }
                catch
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(DirAndFiles[i]);
                    string NewDirPathInDir = $@"{DestinationPath}\{dirInfo.Name}".ToString();
                    if (Directory.Exists(NewDirPathInDir))
                    {
                        Directory.Delete(NewDirPathInDir);
                    }
                    string NewDirPath = Directory.CreateDirectory(NewDirPathInDir).ToString();
                    cpDirCommand(DirAndFiles[i], NewDirPath);
                }

            }
        }

    }
}
