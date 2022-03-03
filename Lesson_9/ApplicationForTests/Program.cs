using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lesson_9;
namespace ApplicationForTests
{
    internal class Programm
    {

        public class Person
        {
            public int age { get; set; }

            public int name { get; set; }

            public int surname { get; set; }

            public class student
            {
                public int Class { get; set; }

                public int Course { get; set; }



            }
        }

        static void Main(string[] args)
        {

            Stopwatch sw = new Stopwatch();
            sw.Restart();
            cpDirCommand(@"C:\DRIVER\Bluetooth", @"C:\Program Files (x86)\Fallout4New");
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
                        File.Delete(NewfilePathInDir);
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
