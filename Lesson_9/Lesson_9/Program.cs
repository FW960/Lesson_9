using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Lesson_9
{
    [Serializable]
    public class PageSize
    {
        public int pageSize { get; set; } = 5;

    }


    internal class Program
    {
        static void ExceptionsWriter(Exception ex)
        {
            File.AppendAllText(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\ErrorsFile.txt", ex.Message);
        }

        static void Main()
        {
            PageSize pagesize = new PageSize();

            XmlSerializer serializer = new XmlSerializer(typeof(PageSize));

            FileStream fileStream = new FileStream(@"C:\Users\windo\source\repos\Lesson_9\Lesson_9\Lesson_9\App.config", FileMode.OpenOrCreate);

            pagesize = serializer.Deserialize(fileStream) as PageSize;

            Menu(pagesize.pageSize);

        }

        static void Menu(int PageSize)
        {
            while (true)
            {
                int PageNum = 0;

                string userCommand = Console.ReadLine();

                string[] UserCommand = userCommand.Split(' ');

                try
                {
                    PageNum = Convert.ToInt32(UserCommand[2]);
                }
                catch (Exception ex)
                {

                }

                switch (UserCommand[0])
                {
                    case "lsDir":
                        lookCommand(UserCommand[1], PageSize, PageNum);
                        break;
                    case "lookPreviousDir":
                        break;
                    case "copy":
                        break;
                    case "delete":
                        break;
                    case "info":
                        break;
                    case "create":
                        break;
                    case "saveAndExit":
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                        break;
                }
            }



        }
        static void lookCommand(string path, int PageSize, int PageNum)
        {

            try
            {
                string[] PathInArray = Directory.EnumerateFileSystemEntries(path).ToArray();

                for (int i = PageNum * PageSize; i < (PageNum * PageSize) + PageSize; i++)
                {
                    try
                    {

                        Console.WriteLine(PathInArray[i]);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Путь не найден.");

                ExceptionsWriter(ex);
            }


        }


    }

}
