using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Lesson_9
{
    [Serializable]
    public class Config
    {
        public int pageSize { get; set; } = 5;// Длина поля по умолчанию, которая записывается в Path.json.
    }

    public class Program
    {
        static void ExceptionsWriter(Exception ex)
        {
            File.AppendAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\ErrorsFile.txt", Environment.NewLine + ex.Message);


        }

        public static void Main()
        {
            Config config = new Config();

            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            FileStream fileStream = new FileStream(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\App.config", FileMode.OpenOrCreate);

            config = serializer.Deserialize(fileStream) as Config; //Десериализуем из файла конфигурации приложения размер страницы

            string DirectoryPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json"); //Читаем путь из файла Json, который сохранился при выходе.

            if (Directory.Exists(DirectoryPath))
            {
                lsCommand(DirectoryPath, config.pageSize, 0);
            }

            Menu(config.pageSize, DirectoryPath);



        }

        public static void Menu(int PageSize, string DirectoryPath)
        {
            while (true)
            {
                File.WriteAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json", DirectoryPath); // Записываем в Json файл путь, который сохранится в нем при выходе.

                int PageSplit = 0;

                string userCommand = Console.ReadLine(); // Вся эта огромная логика нужна для того, чтобы команды, файлы и директории можно было бы разделять одним пробелом, если там допустим будет папка C:\Program Files (x86), в которой несколько пробелов.

                string[] FirstUserCommand = userCommand.Split(':');

                string[] UserCommand = FirstUserCommand[0].Split(' ');

                string diskPath = string.Empty;

                string DiskPlusUserCommand = string.Empty;

                if (FirstUserCommand.Length > 1)
                {
                    try
                    {
                        diskPath = ($"{UserCommand[1]}:");

                        DiskPlusUserCommand = ($"{diskPath}{FirstUserCommand[1]}");
                    }
                    catch
                    {
                        Exception ex = new Exception("Error in entering path. User haven't entered pass correctly");

                        Console.WriteLine("Enter path correctly");

                        ExceptionsWriter(ex);

                        continue;
                    }
                }


                if (FirstUserCommand.Length > 2)
                {
                    string LastDiskPlucUserCommand = ((FirstUserCommand[1][FirstUserCommand[1].Length - 2].ToString() + (FirstUserCommand[1][FirstUserCommand[1].Length - 1].ToString())));

                    string[] AnotherDisk = FirstUserCommand[1].Split(LastDiskPlucUserCommand);

                    FirstUserCommand[0] = UserCommand[0];

                    FirstUserCommand[1] = $"{diskPath}{AnotherDisk[0]}";

                    FirstUserCommand[2] = $"{diskPath}{FirstUserCommand[2]}";
                }
                else if (FirstUserCommand.Length == 2)
                {

                    FirstUserCommand[0] = UserCommand[0];

                    string[] PageSplitter = DiskPlusUserCommand.Split(" /p");

                    try
                    {
                        PageSplit = Convert.ToInt32(PageSplitter[1]);
                    }
                    catch
                    {

                    }
                    FirstUserCommand[1] = PageSplitter[0];


                }


                switch (FirstUserCommand[0])
                {
                    case "ls":
                        try
                        {
                            DirectoryPath = lsCommand(FirstUserCommand[1], PageSize, PageSplit);
                        }
                        catch
                        {
                            Exception ex = new Exception("Error incorrect using of ls command. User should type directory path next to ls command");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Type directory path next to ls command");
                        }

                        break;
                    case "cpFile":
                        try
                        {
                            cpFileCommand(FirstUserCommand[1].ToString(), FirstUserCommand[2].ToString());
                        }
                        catch
                        {
                            Exception ex = new Exception("Error incorrect using of cpFile command. User should type files path next to cpFile command");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Type files path next to cpFile command");
                        }

                        break;
                    case "cpDir":

                        try
                        {
                            (FirstUserCommand[1], FirstUserCommand[2]) = additionTocpDirCommand(FirstUserCommand[1].ToString(), FirstUserCommand[2].ToString());

                            if (FirstUserCommand[1] == string.Empty || FirstUserCommand[2] == string.Empty)
                            {
                                break;
                            }

                            cpDirCommand(FirstUserCommand[1].ToString(), FirstUserCommand[2].ToString());

                            Console.WriteLine("Directory succsesfully coppied.");
                        }
                        catch
                        {
                            Exception ex = new Exception("Error incorrect using of cpDir command. User should type directories path next to cpDir command");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Type files path next to cpDir command");
                        }
                        break;
                    case "rmDir":
                        try
                        {
                            rmDirCommand(FirstUserCommand[1].ToString());
                        }
                        catch
                        {
                            Exception ex = new Exception("Error incorrect using of rmDir command. User should type directory path next to rmDir command");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Type directory path next to rmDir command");
                        }
                        break;
                    case "rmFile":
                        try
                        {
                            rmFileCommand(FirstUserCommand[1].ToString());
                        }
                        catch
                        {
                            Exception ex = new Exception("Error incorrect using of rmFile command. User should type file path next to rmFile command");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Type file path next to rmFile command");
                        }
                        break;
                    case "help":
                        Console.WriteLine(@"CommandsHelp
ls - Look all directory files and folders. Write next to this command Directory you would like to look at.
cpFile - copy file. Firsly write next to this command source file and the destination file.
cpDir - copy directory. Firstly write next to this command source directory path and then destination directory path.
rmDir - fully delete directory. Write next to this command directory path you would like to delete.
rmFile - delete file. Write next to this command file path you would like to delete.");
                        break;
                    case "create":
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Can not identify the command.");
                        break;
                }
            }



        }
        public static string lsCommand(string DirectoryPath, int PageSize, int PageNum)
        {

            if (Directory.Exists(DirectoryPath))
            {
                string[] PathInArray = Directory.EnumerateFileSystemEntries(DirectoryPath).ToArray();

                if (PathInArray.Length == 0)
                {
                    Console.WriteLine($"Directory {DirectoryPath} is empty");
                }

                for (int i = PageNum * PageSize; i < (PageNum * PageSize) + PageSize; i++)
                {
                    try
                    {
                        Console.WriteLine(PathInArray[i]);
                    }
                    catch (IndexOutOfRangeException ex)
                    {

                        return DirectoryPath;

                    }
                }
                return DirectoryPath;
            }
            else if (File.Exists(DirectoryPath))
            {
                Exception ex = new Exception($"Error lsCommand. Can't see directories inside of file {DirectoryPath}.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't see directories inside of file {DirectoryPath}.");

                string DirPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json");

                return DirPath;
            }
            else
            {
                Exception ex = new Exception($"Error lsCommand. Directory {DirectoryPath} doesn't exist");

                ExceptionsWriter(ex);

                Console.WriteLine($"Directory {DirectoryPath} doesn't exist");

                string DirPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json");

                return DirPath;
            }



        }
        public static void cpFileCommand(string SourceFile, string DestinationFile)
        {
            if (Directory.Exists(SourceFile))
            {
                Exception ex = new Exception($"Error cpFile command. Can't copy directory {SourceFile} usind cpFile command.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't copy directory {SourceFile} using cpFile command.");
                return;
            }
            if (Directory.Exists(DestinationFile))
            {
                Exception ex = new Exception($"Error cpFile command. Can't copy directory {DestinationFile} usind cpFile command.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't copy directory {DestinationFile} using cpFile command.");
                return;
            }

            if (SourceFile == DestinationFile)
            {
                Exception ex = new Exception($"Error cpFile command. Can't copy file {SourceFile} to into oneself");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't copy file {SourceFile} to into oneself");

                return;
            }


                if (!File.Exists(SourceFile))
                {
                    {
                        Exception ex = new Exception($"Error cpFile command. Source file {SourceFile} doesn't exits.");

                        ExceptionsWriter(ex);

                        Console.WriteLine($"Source file {SourceFile} doesn't exits.");
                        return;
                    }
                    
                }
                else if (File.Exists(SourceFile))
                {
                    if (File.Exists(DestinationFile))
                    {
                        File.Delete(DestinationFile);
                    }
                    File.Copy(SourceFile, DestinationFile);
                    Console.WriteLine($"File {SourceFile} sucessefuly copied to {DestinationFile}.");
                }

        }

        public static (string SourcePath, string DestinationPath) additionTocpDirCommand(string SourcePath, string DestinationPath)
        {

            if (File.Exists(SourcePath))
            {
                Exception ex = new Exception($"Error cpDir command. Can't copy file {SourcePath} using cpDir command.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't copy file {SourcePath} using cpDir command.");

                return (string.Empty, string.Empty);
            }
            if (File.Exists(DestinationPath))
            {
                Exception ex = new Exception($"Error cpDir command. Can't copy file {DestinationPath} using cpDir command.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't copy file {DestinationPath} using cpDir command.");

                return (string.Empty, string.Empty);
            }

            if (!Directory.Exists(SourcePath))
            {
                Exception ex = new Exception($"Error cpDir command. Source directory {SourcePath} doesnt't exist.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Source directory {SourcePath} doesn't exist");

                return (string.Empty, string.Empty);
            }
            if (!Directory.Exists(DestinationPath))
            {
                Exception ex = new Exception($"Error cpDir command. Destination directory {DestinationPath} doesnt't exist.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Destination directory {DestinationPath} doesn't exist");

                return (string.Empty, string.Empty);
            }
            return (SourcePath, DestinationPath);



        }

        public static void cpDirCommand(string SourcePath, string DestinationPath)
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
                        rmDirCommand(NewDirPathInDir);
                    }
                    string NewDirPath = Directory.CreateDirectory(NewDirPathInDir).ToString();
                    cpDirCommand(DirAndFiles[i], NewDirPath);
                }
                

            }
        }
        public static void rmDirCommand(string DirectoryPath)
        {
            try
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Exception ex = new Exception("Error rmDir command. Directory doesn't exist.");

                    ExceptionsWriter(ex);

                    Console.WriteLine("Directory doesn't exist.");

                    return;
                }
            }
            catch
            {
                Exception ex = new Exception("Error rmDir command. Can't delete file using rmDir command.");

                ExceptionsWriter(ex);

                Console.WriteLine("Can't delete file using rmDir command.");

                return;
            }
            string[] allderictories = Directory.EnumerateFileSystemEntries(DirectoryPath).ToArray();

            for (int i = 0; i < DirectoryPath.Length - 1; i++)
            {
                try
                {
                    File.SetAttributes(allderictories[i], FileAttributes.Normal);

                    File.Delete(allderictories[i]);

                }
                catch
                {
                    try
                    {

                        Directory.Delete(DirectoryPath);

                        return;
                    }
                    catch
                    {
                        rmDirCommand(allderictories[i]);
                    }
                }
            }
        }
        public static void rmFileCommand(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    Console.WriteLine("File succsesfully deleted.");
                }
                else
                {
                    Exception ex = new Exception("Error rmFile command. File doesn't exist.");

                    ExceptionsWriter(ex);

                    Console.WriteLine("File doesn't exist");

                    return;
                }

            }
            catch
            {
                Exception ex = new Exception("Error rmFile command. Can't delete directory using rmFile command.");

                ExceptionsWriter(ex);

                Console.WriteLine("Can't delete directory using rmFile command.");

                return;


            }
        }



    }

}
