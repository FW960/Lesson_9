using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lesson_9;
namespace ApplicationForTests
{
    internal class Programm
    {



        static void Main(string[] args)
        {

            string DirectoryPath = directoryExistCheck(Console.ReadLine());

            if (DirectoryPath == string.Empty)
            {
                return;
            }

            Single a = infoDirCommand(DirectoryPath);

            Console.WriteLine($"{a/(1024*1024)} mb");
        }

        public static string directoryExistCheck(string DirectoryPath)
        {
            if (File.Exists(DirectoryPath))
            {
                Exception ex = new Exception($"Error infoDir command. Cant't see file {DirectoryPath} info using dirInfo command.");

                Console.WriteLine($"Cant't see file {DirectoryPath} info using dirInfo command.");

                return string.Empty;
            }
            else if (!Directory.Exists(DirectoryPath))
            {
                Exception ex = new Exception($"Error infoDir command. Directory {DirectoryPath} doesn't exist");

                Console.WriteLine($"Directory {DirectoryPath} doesn't exist");

                return string.Empty;
            }
            else 
            {
                return DirectoryPath;
            }
        }
        public static long infoDirCommand(string filePath)
        {
            string[] DirAndFile = Directory.EnumerateFileSystemEntries(filePath).ToArray();

            long NewFileSize = 0;

            long NewDirectorySize = 0;

            for (int i = 0; i < DirAndFile.Length; i++)
            {
                try
                {
                    if (File.Exists(DirAndFile[i]))
                    {
                        FileStream fileStream = new FileStream(DirAndFile[i], FileMode.OpenOrCreate, FileAccess.Read);

                        long FileSize = fileStream.Length;

                        NewFileSize = NewFileSize + FileSize;
                    }
                    else
                    {
                        long DirectorySize = infoDirCommand(DirAndFile[i]);

                        NewDirectorySize = NewDirectorySize + DirectorySize;
                    }
                }
                catch
                {
                    Console.WriteLine($"Acceses to file {DirAndFile[i]} is denied. Continue without it");
                }

            }
            return (NewFileSize + NewDirectorySize);


        }



    }
}
