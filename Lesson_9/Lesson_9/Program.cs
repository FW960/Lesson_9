using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace TaskManager
{
    [Serializable]
    public class Config
    {
        public int pageSize { get; set; }// Длина поля, которая записывается в Path.json
    }

    public class Program
    {
        static void ExceptionsWriter(Exception ex)
        {
            if (!File.Exists(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\ErrorsFile.txt"))
            {
                FileStream fs = new FileStream(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\ErrorsFile.txt", FileMode.OpenOrCreate);

                fs.Close();

                Exception fileError = new Exception("Errors file error. File have been deleted.");

                ExceptionsWriter(fileError);
            }
            else
            {
                File.AppendAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\ErrorsFile.txt", Environment.NewLine + ex.Message);

            }

        }

        public static void Main()
        {
            Config config = new Config();

            string DirectoryPath = string.Empty;

            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            if (!File.Exists(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\App.config")) //Оператор if на случай если файл полностью удален.
            {
                Exception ex = new Exception("Config file error. File have been deleted.");

                ExceptionsWriter(ex);

                FileStream fs = new FileStream(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\App.config", FileMode.OpenOrCreate);

                fs.Close();
            }
            if (File.Exists(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\App.config"))
            {
                FileStream fileStream = new FileStream(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\App.config", FileMode.Open, FileAccess.ReadWrite);

                try //На тот случай если в файл внесены изменения, которые невозможно десериализовать
                {
                    config = serializer.Deserialize(fileStream) as Config; //Десериализуем из файла конфигурации приложения размер страницы

                    fileStream.Close();
                }
                catch
                {
                    Exception ex = new Exception("Config file error. File wasn't in correct format.");

                    ExceptionsWriter(ex);

                    config.pageSize = 10;

                    serializer.Serialize(fileStream, config);

                    fileStream.Close();
                }
            }

            if (File.Exists(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json"))
            {
                DirectoryPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json"); //Читаем путь из файла Json, который сохранился при выходе.
            }
            else // Создаем новый на тот случай, если он был удален.
            {
                FileStream fs = new FileStream(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json", FileMode.Create);

                fs.Close();

                Exception ex = new Exception("Path file error. File have been deleted.");

                ExceptionsWriter(ex);
            }

            if (DirectoryPath != string.Empty)
            {
                ls(DirectoryPath, config.pageSize, 0);
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


                    diskPath = ($"{UserCommand[1]}:");

                    DiskPlusUserCommand = ($"{diskPath}{FirstUserCommand[1]}");

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
                            if (FirstUserCommand.Length == 2)
                            {
                                DirectoryPath = ls(DirectoryPath, PageSize, PageSplit);

                            }else
                            {
                                Exception ex = new Exception("Error ls command. Incorrect using of ls command.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of ls command.");
                            }

                        }
                        catch
                        {
                            Exception ex = new Exception("Error ls command. Incorrect using of ls command.");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Incorrect using of ls command.");
                        }

                        break;
                    case "cpFile":
                        try
                        {
                            if (FirstUserCommand.Length == 3)
                            {
                                cpFile(FirstUserCommand[1].ToString(), FirstUserCommand[2].ToString());
                            }else
                            {
                                Exception ex = new Exception("Error cpFile command. Incorrect using of cpFile command.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of cpFile command.");
                            }

                        }
                        catch
                        {
                            Exception ex = new Exception("Error cpFile command. Incorrect using of cpFile command.");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Incorrect using of cpFile command.");
                        }

                        break;
                    case "cpDir":

                        try
                        {

                            if (FirstUserCommand.Length == 3)
                            {
                                cpDir(FirstUserCommand[1].ToString(), FirstUserCommand[2].ToString());
                            }else
                            {
                                Exception ex = new Exception("Error cpDir command. Incorrect using of cpDir command.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of cpDir command.");
                            }


                        }
                        catch
                        {
                            Exception ex = new Exception("Error cpDir command. Incorrect using of cpDir command.");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Incorrect using of cpDir command.");

                        }
                        
                        break;
                    case "rmDir":

                        try
                        {
                            if (FirstUserCommand.Length == 2)
                            {
                                rmDir(FirstUserCommand[1].ToString());
                            }else
                            {
                                Exception ex = new Exception("Error rmDirCommand. Incorrect using of rmDir command");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of rmDir command.");
                            }

                        }
                        catch
                        {
                            Exception ex = new Exception("Error rmDirCommand. Incorrect using of rmDir command");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Incorrect using of rmDir command.");

                        }
                        
                        break;
                    case "rmFile":
                        try
                        {
                            if (FirstUserCommand.Length == 2)
                            {
                                rmFile(FirstUserCommand[1].ToString());
                            }else
                            {
                                Exception ex = new Exception("Error rmFile command. Incorrect using of infoFile command.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of infoFile command.");
                            }


                        }
                        catch 
                        {
                            Exception ex = new Exception("Error rmFile command. Incorrect using of infoFile command.");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Incorrect using of infoFile command.");
                        }
                        break;
                    case "help":
                        Console.WriteLine(@"CommandsHelp
Use spacebar button to separate commands, directories paths and files paths and paging.

For paging use type ls command, then directory path and finally /p'number of page'.

ls - Look all directory files and folders. Write next to this command directory path you would like to look at.
cpFile - copy file. Firsly write next to this command source file path and the destination file path.
cpDir - copy directory. Firstly write next to this command source directory path and then destination directory path.
rmDir - fully delete directory. Write next to this command directory path you would like to delete.
rmFile - delete file. Write next to this command file path you would like to delete.
infoFile - get file size and attributtes. Write next to this command file path you would like to look at info.
infoDir - get file size and attributes. Write next to this command directory path you would like to look info.");
                        break;
                    case "infoFile":
                        try
                        {
                            if (FirstUserCommand.Length == 2)
                            {
                                infoFile(FirstUserCommand[1]);
                            }else
                            {
                                Exception ex = new Exception("Error infoFile command. Incorrect using of infoFile command.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of infoFile command.");
                            }

                        }
                        catch
                        {
                            Exception ex = new Exception("Error infoFile command. Incorrect using of infoFile command.");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Incorrect using of infoFile command.");
                        }
                        break;
                    case "infoDir":
                        try
                        {
                            if (FirstUserCommand.Length == 2)
                            {
                                infoDir(FirstUserCommand[1]);
                            }else
                            {
                                Exception ex = new Exception("Error infoDir command. Incorrect using of infoDir command.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of infoDir command.");
                            }

                        }
                        catch
                        {
                            Exception ex = new Exception("Error infoDir command. Incorrect using of infoDir command.");

                            ExceptionsWriter(ex);
                            Console.WriteLine("Incorrect using of infoDir command.");
                        }

                        break;
                    case "crDir":
                        try
                        {
                            if (FirstUserCommand.Length == 2)
                            {
                                crDir(FirstUserCommand[1]);
                            }else
                            {
                                Exception ex = new Exception("Error crDir command. Incorrect using of crDir command.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of crDir command.");
                            }

                        }
                        catch
                        {
                            Exception ex = new Exception("Error crDir command. Incorrect using of crDir command.");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Incorrect using of crDir command.");
                        }
                        break;
                    case "crFile":
                        try
                        {
                            if (FirstUserCommand.Length == 2)
                            {
                                crFile(FirstUserCommand[1]);
                            }else
                            {
                                Exception ex = new Exception("Error crFile command. Incorrect using of crFile command.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Incorrect using of crFile command.");
                            }


                        }
                        catch
                        {
                            Exception ex = new Exception("Error crFile command. Incorrect using of crFile command.");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Incorrect using of crFile command.");
                        }
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                        try
                        {
                            if ((FirstUserCommand[0])[0] == ' ' || FirstUserCommand[0][FirstUserCommand[0].Length - 1] == ' ')
                            {
                                Exception ex = new Exception("Error command choice. Correctly use command splitter.");

                                ExceptionsWriter(ex);

                                Console.WriteLine("Correctly use command splitter.");


                            }
                            else
                            {
                                Exception exAnother = new Exception("Error command choice. Correctly write command.");

                                ExceptionsWriter(exAnother);

                                Console.WriteLine("Correctly write command.");
                            }
                        }
                        catch
                        {
                            Exception ex = new Exception("Error command choice. Correctly use command splitter.");

                            ExceptionsWriter(ex);

                            Console.WriteLine("Correctly use command splitter.");
                        }

                        break;
                }
            }



        }
        public static string ls(string DirectoryPath, int PageSize, int PageNum)
        {
            if (DirectoryPath[0] == ':' || DirectoryPath[DirectoryPath.Length - 1] == ' ')
            {
                Exception ex = new Exception("Error lsCommand. Incorrect command and directory split.");

                ExceptionsWriter(ex);

                Console.WriteLine("Correctly split command and directory.");

                string DirPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json.");

                return DirPath;
            }
            if (Directory.Exists(DirectoryPath))
            {
                try
                {
                    string[] PathInArray = Directory.EnumerateFileSystemEntries(DirectoryPath).ToArray();

                    if (PathInArray.Length == 0)
                    {
                        Console.WriteLine($"Directory {DirectoryPath} is empty.");
                    }

                    for (int i = PageNum * PageSize; i < (PageNum * PageSize) + PageSize; i++)
                    {
                        try
                        {
                            Console.WriteLine(PathInArray[i]);
                        }
                        catch (IndexOutOfRangeException)
                        {

                            return DirectoryPath;

                        }
                    }
                    return DirectoryPath;
                }
                catch
                {
                    Exception ex = new Exception($"Error ls command. Access to {DirectoryPath} denied");

                    ExceptionsWriter(ex);

                    Console.WriteLine($"Access to {DirectoryPath} denied");

                    string DirPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json.");

                    return DirPath;
                }
            }
            else if (File.Exists(DirectoryPath))
            {
                Exception ex = new Exception($"Error lsCommand. Can't see directories inside of file {DirectoryPath}.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't see directories inside of file {DirectoryPath}.");

                string DirPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json.");

                return DirPath;
            }
            else
            {
                Exception ex = new Exception($"Error lsCommand. Directory {DirectoryPath} doesn't exist.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Directory {DirectoryPath} doesn't exist.");

                string DirPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json.");

                return DirPath;
            }




        }
        public static void cpFile(string SourceFile, string DestinationFile)
        {
            if (SourceFile[0] == ':' || DestinationFile[0] == ':' || SourceFile[SourceFile.Length - 1] == ' ' || DestinationFile[DestinationFile.Length - 1] == ' ')
            {
                Exception ex = new Exception("Error cpFile. Incorrect command and directory split.");

                ExceptionsWriter(ex);

                Console.WriteLine("Correctly split command and file.");

                return;
            }
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
                Exception ex = new Exception($"Error cpFile command. Can't copy file {SourceFile} into oneself.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't copy file {SourceFile} into oneself.");

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
        public static void cpDir(string SourcePath, string DestinationPath)
        {
            try
            {
                (SourcePath, DestinationPath) = DirectoryCheckerAdditionTocpDirCommand(SourcePath, DestinationPath);

                if (SourcePath == string.Empty || DestinationPath == string.Empty)
                {
                    return;
                }
                CopyingcpDirCommand(SourcePath, DestinationPath);

                Console.WriteLine($"Source directory {SourcePath} succesfully copied to {DestinationPath}");
            }
            catch
            {
                Exception ex = new Exception("Error cpDir command. Incorrect using of cpDir command.");

                ExceptionsWriter(ex);

                Console.WriteLine("Incorrect using of cpDir command.");

                return;
            }
        }
        public static (string SourcePath, string DestinationPath) DirectoryCheckerAdditionTocpDirCommand(string SourcePath, string DestinationPath)
        {
            if (SourcePath[0] == ':' || DestinationPath[0] == ':' || SourcePath[SourcePath.Length - 1] == ' ' || DestinationPath[DestinationPath.Length - 1] == ' ')
            {
                Exception ex = new Exception("Error cpDir. Incorrect command and directories split.");

                ExceptionsWriter(ex);

                Console.WriteLine("Correctly split command and directories.");

                return (string.Empty, string.Empty);
            }
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

                Console.WriteLine($"Source directory {SourcePath} doesn't exist.");

                return (string.Empty, string.Empty);
            }
            if (!Directory.Exists(DestinationPath))
            {
                Exception ex = new Exception($"Error cpDir command. Destination directory {DestinationPath} doesnt't exist.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Destination directory {DestinationPath} doesn't exist.");

                return (string.Empty, string.Empty);
            }
            if (SourcePath == DestinationPath)
            {
                Exception ex = new Exception($"Error cpDir command. Can't copy directory {SourcePath} into onself.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't copy directory {SourcePath} into onself.");

                return (string.Empty, string.Empty);
            }
            else
            {
                return (SourcePath, DestinationPath);
            }



        }

        public static void CopyingcpDirCommand(string SourcePath, string DestinationPath)
        {
            

            string[] DirAndFiles = Directory.EnumerateFileSystemEntries(SourcePath).ToArray();

            for (int i = 0; i < DirAndFiles.Length; i++)
            {
                try
                {
                    string NewfilePathInDir = @$"{ DestinationPath}\{Path.GetFileName(DirAndFiles[i])}";
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
                        DeletingrmDirCommand(NewDirPathInDir);
                    }
                    string NewDirPath = Directory.CreateDirectory(NewDirPathInDir).ToString();
                    CopyingcpDirCommand(DirAndFiles[i], NewDirPath);
                }


            }
        }

        public static void rmDir(string DirectoryPath)
        {

            try
            {
                DirectoryPath = (DirectoryCheckerAdditiontormDirCommand(DirectoryPath));

                if (DirectoryPath == string.Empty)
                {
                    return;
                }

                DeletingrmDirCommand(DirectoryPath);

                Console.WriteLine($"Directory {DirectoryPath} succesfully deleted.");
            }  catch
            {
                Exception ex = new Exception("Error rmDir command. Incorrect using of rmDir command.");

                ExceptionsWriter(ex);

                Console.WriteLine("Incorrect using of rmDir command.");

                return;
            }
        }
        public static string DirectoryCheckerAdditiontormDirCommand(string DirectoryPath)
        {
            
                if (DirectoryPath[0] == ':' || DirectoryPath[DirectoryPath.Length - 1] == ' ')
                {
                    Exception ex = new Exception("Error rmDir. Incorrect command and directory split.");

                    ExceptionsWriter(ex);

                    Console.WriteLine("Correctly split command and directory.");

                    return string.Empty;
                }
                if (File.Exists(DirectoryPath))
                {
                    Exception ex = new Exception("Error rmDir command. Can't delete file using rmDir command.");

                    ExceptionsWriter(ex);

                    Console.WriteLine("Can't delete file using rmDir command.");

                    return string.Empty;
                }
                if (!Directory.Exists(DirectoryPath))
                {
                    Exception ex = new Exception("Error rmDir command. Directory doesn't exist.");

                    ExceptionsWriter(ex);

                    Console.WriteLine("Directory doesn't exist.");

                    return string.Empty;
                }

                else
                {
                    return DirectoryPath;
                }
            
            
        }

        public static void DeletingrmDirCommand(string DirectoryPath)
        {


            string[] allderictories = Directory.EnumerateFileSystemEntries(DirectoryPath).ToArray();

            for (int i = 0; i < DirectoryPath.Length; i++)
            {
                try
                {
                    File.SetAttributes(allderictories[i], FileAttributes.Normal);

                    FileStream fs = File.OpenRead(allderictories[i]);

                    fs.Close();

                    File.Delete(allderictories[i]);

                }
                catch (IndexOutOfRangeException)
                {

                    Directory.Delete(DirectoryPath);

                    return;

                }
                catch
                {
                    try
                    {

                        Directory.Delete(DirectoryPath);

                    }
                    catch
                    {
                        DeletingrmDirCommand(allderictories[i]);
                    }
                }
            }
        }
        public static void rmFile(string FilePath)
        {
            try
            {
                if (FilePath[0] == ':' || FilePath[FilePath.Length - 1] == ' ')
                {
                    Exception ex = new Exception("Error rmFile. Incorrect command and file split.");

                    ExceptionsWriter(ex);

                    Console.WriteLine("Correctly split command and file.");

                    return;
                }
                else if (File.Exists(FilePath))
                {
                    FileStream fs = File.OpenRead(FilePath);

                    fs.Close();

                    File.Delete(FilePath);
                    Console.WriteLine("File succsesfully deleted.");

                    return;
                }
                else if (Directory.Exists(FilePath))
                {
                    Exception ex = new Exception($"Error rmFile command. Can't delete directory {FilePath} using rmFile command.");

                    ExceptionsWriter(ex);

                    Console.WriteLine($"Can't delete directory {FilePath} using rmFile command.");

                    return;
                }
                if (!File.Exists(FilePath))
                {
                    Exception ex = new Exception($"Error rmFile command. File {FilePath} doesn't exist.");

                    ExceptionsWriter(ex);

                    Console.WriteLine($"File {FilePath} doesn't exist.");

                    return;
                }
            }
            catch
            {
                Exception ex = new Exception($"Error rmFile command. Access to file {FilePath} denied.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Access to file {FilePath} denied.");
            }

        }

        public static void infoFile(string filePath)
        {
            if (filePath[0] == ':' || filePath[filePath.Length - 1] == ' ')
            {
                Exception ex = new Exception("Error infoFile. Incorrect command and file split.");

                ExceptionsWriter(ex);

                Console.WriteLine("Correctly split command and file.");

                return;
            }
            if (Directory.Exists(filePath))
            {
                Exception ex = new Exception($"Error info command. Can't see directory {filePath} info using infoFile command.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't see directory {filePath} info using infoFile command.");

                return;
            }

            if (File.Exists(filePath))
            {
                FileInfo file = new FileInfo(filePath);

                double a = Convert.ToDouble(file.Length);

                Console.WriteLine(@$"Path {filePath}.
{file.Name} size: {a / 1000} kb.
{file.Name} attributes: {file.Attributes}.");
            }


            if (!File.Exists(filePath))
            {
                Exception ex = new Exception($"Error info command. Can't find path {filePath}.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't find path to file {filePath}.");

                return;
            }


        }

        public static void infoDir(string InfoDirectory)
        {
            InfoDirectory = DirectoryCheckerAdditioninfoDirCommand(InfoDirectory);

            if (InfoDirectory == string.Empty)
            {
                return;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(InfoDirectory);

            Console.WriteLine($@"Type any button to calculate size of {directoryInfo.Name} and see attributes. 
Type 0 if you changed your mind and want to exit from infoDir command. Calculating may take sometime.");
            string UserChoice = Console.ReadLine();
            Console.WriteLine("Calculating directory size.");

            if (UserChoice == "0")
            {
                return;

            }

            Stopwatch sw = new Stopwatch();

            sw.Restart();

            Single DirSize = CalculationinfoDirCommand(InfoDirectory);

            sw.Stop();

            Console.WriteLine();

            Console.WriteLine(@$"Path {InfoDirectory}
{directoryInfo.Name} size: {DirSize / 1000} KB. 
{directoryInfo.Name} attributes: {directoryInfo.Attributes}. 
Time elapsed {sw.ElapsedMilliseconds} ms.");

        }

        public static string DirectoryCheckerAdditioninfoDirCommand(string DirectoryPath)
        {
            if (DirectoryPath[0] == ':' || DirectoryPath[DirectoryPath.Length - 1] == ' ')
            {
                Exception ex = new Exception("Error infoDir. Incorrect command and directory split.");

                ExceptionsWriter(ex);

                Console.WriteLine("Correctly split command and directory.");

                return string.Empty;
            }

            if (File.Exists(DirectoryPath))
            {
                Exception ex = new Exception($"Error infoDir command. Cant't see file {DirectoryPath} info using dirInfo command.");

                Console.WriteLine($"Cant't see file {DirectoryPath} info using dirInfo command.");

                return string.Empty;
            }
            if (!Directory.Exists(DirectoryPath))
            {
                Exception ex = new Exception($"Error infoDir command. Directory {DirectoryPath} doesn't exist.");

                Console.WriteLine($"Directory {DirectoryPath} doesn't exist.");

                return string.Empty;
            }
            if(Directory.Exists(DirectoryPath))
            {
                return DirectoryPath;
            }else
            {
                return string.Empty;
            }
        }
        public static long CalculationinfoDirCommand(string DirPath)
        {
            string[] DirAndFile = Directory.EnumerateFileSystemEntries(DirPath).ToArray();

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
                        long DirectorySize = CalculationinfoDirCommand(DirAndFile[i]);

                        NewDirectorySize = NewDirectorySize + DirectorySize;
                    }
                }
                catch
                {
                    Console.WriteLine($"Acceses to file {DirAndFile[i]} is denied. Continuing without it.");
                }

            }
            return (NewFileSize + NewDirectorySize);


        }
        static void crFile(string FilePath)
        {

            if (FilePath[0] == ':' || FilePath[FilePath.Length - 1] == ' ')
            {
                Exception ex = new Exception("Error crFile. Incorrect command and file split.");

                ExceptionsWriter(ex);

                Console.WriteLine("Correctly split command and file.");

                return;
            }
            if (Directory.Exists(FilePath))
            {
                Exception ex = new Exception($"Error crFile command. Directory {FilePath} already exists.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Directory {FilePath} already exists. Change file name.");

                return;
            }
            if (File.Exists(FilePath))
            {
                Exception ex = new Exception($"Error crFile command. File {FilePath} already exists in this path.");

                ExceptionsWriter(ex);

                Console.WriteLine($"File {FilePath} already exists in this path.");

                return;
            }
            else
            {
                FileStream fileCreation = new FileStream(FilePath, FileMode.Create);

                fileCreation.Close();

                Console.WriteLine("File succesfully created.");

                return;
            }

        }
        static void crDir(string DirectoryPath)
        {
            if (DirectoryPath[0] == ':' || DirectoryPath[DirectoryPath.Length - 1] == ' ')
            {
                Exception ex = new Exception("Error crFile. Incorrect command and directory split.");

                ExceptionsWriter(ex);

                Console.WriteLine("Correctly split command and directory.");

                return;
            }
            if (Directory.Exists(DirectoryPath))
            {
                Exception ex = new Exception($"Error crDir command. Can't create already existing directory {DirectoryPath}.");

                ExceptionsWriter(ex);

                Console.WriteLine($"Can't create already existing directory {DirectoryPath}.");

                return;
            }
            else
            {
                Directory.CreateDirectory(DirectoryPath);

                Console.WriteLine("Directory succesfully created.");

                return;
            }

        }



    }

}
