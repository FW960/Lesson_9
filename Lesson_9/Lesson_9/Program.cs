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
        public int pageSize { get; set; } = 5;
    }
    internal class Program
    {
        static void ExceptionsWriter(Exception ex)
        {
            File.AppendAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\ErrorsFile.txt", ex.Message);
        }
        static int DirectoryExist(string DirectoryPath)
        {
            try
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Console.WriteLine("Directory isnt exist");
                    Exception ex = new Exception("Directory isnt exist");
                    ExceptionsWriter(ex);
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        static void Main()
        {
            Config config = new Config();

            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            FileStream fileStream = new FileStream(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\App.config", FileMode.OpenOrCreate);

            config = serializer.Deserialize(fileStream) as Config; //Десериализуем из файла конфигурации приложения размер страницы

            string DirectoryPath = File.ReadAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json"); //Читаем путь из файла Json, который сохранился при выходе.

            lsCommand(DirectoryPath, config.pageSize, 0);

            Menu(config.pageSize, DirectoryPath);



        }

        static void Menu(int PageSize, string DirectoryPath)
        {
            while (true)
            {
                File.WriteAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\Path.json", DirectoryPath); // Записываем в Json файл путь, который сохранится в нем при выходе.

                int PageNum = 0;

                string userCommand = Console.ReadLine();

                string[] UserCommand = userCommand.Split("  ");

                try
                {
                    PageNum = Convert.ToInt32(UserCommand[2]);
                }
                catch (Exception)
                {

                }

                switch (UserCommand[0])
                {
                    case "ls":
                        try
                        {
                            DirectoryPath = lsCommand(UserCommand[1], PageSize, PageNum);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error. Type full path to directory.");
                            ExceptionsWriter(ex);
                        }
                        break;
                    case "cpFile":
                        try
                        {
                            cpFileCommand(UserCommand[1].ToString(), UserCommand[2].ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error. Correctly write file folder and folder to copy.");
                            ExceptionsWriter(ex);
                        }
                        break;
                    case "cpDir":
                        try
                        {
                            cpDirCommand(UserCommand[1].ToString(), UserCommand[2].ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error. Correctly write Source Directory path and Destination Directory path.");
                            ExceptionsWriter(ex);
                        }
                        break;
                    case "rmDir":
                        try
                        {
                            rmDirCommand(UserCommand[1].ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error. Correctly write Directory.");
                            ExceptionsWriter(ex);
                        }
                        break;
                    case "openFile":
                        break;
                    case "info":
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
        static string lsCommand(string DirectoryPath, int PageSize, int PageNum)
        {
            int checkCode = DirectoryExist(DirectoryPath);
            if (checkCode == 0)
            {
                return string.Empty;
            }
            try
            {
                string[] PathInArray = Directory.EnumerateFileSystemEntries(DirectoryPath).ToArray();

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
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to find path to directory {DirectoryPath}.");

                ExceptionsWriter(ex);

                return string.Empty;
            }
        }
        static void cpFileCommand(string DirectoryPath, string FilePath)
        {
            int checkCode = DirectoryExist(DirectoryPath);
            if (checkCode == 0)
            {
                return;
            }
            try
            {

                File.Copy(DirectoryPath, FilePath);

                Console.WriteLine("File copied");
            }
            catch (SystemException ex)
            {
                Console.WriteLine("Unable to copy. File Already exist in that directory.");

                ExceptionsWriter(ex);
            }

        }

        static void cpDirCommand(string SourcePath, string DestinationPath)
        {
            int checkCode = DirectoryExist(SourcePath);

            if (checkCode == 0)
            {
                return;
            }
            checkCode = DirectoryExist(DestinationPath);

            if (checkCode == 0)
            {
                return;
            }
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
        public static void rmDirCommand(string DirectoryPath)
        {
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


    }

}
