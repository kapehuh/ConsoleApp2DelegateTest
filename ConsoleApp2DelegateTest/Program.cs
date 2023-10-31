using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using static ConsoleApp2DelegateTest.Program;

namespace ConsoleApp2DelegateTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Escape для отмены поиска:");
            Console.WriteLine();
            // === === === поиск наибольшего элемента коллекции
            string[] collection = new string[] { "20", "324", "65" };
            var result = collection.GetMax(_ => float.Parse(_));
            Console.WriteLine($"Результат поиска максимального элемента в коллекции строк[] '20', '324', '65'");
            Console.WriteLine($"collection.GetMax(_ => float.Parse(_)");
            Console.WriteLine($"{result}");
            Console.WriteLine();

            // === === === обход файлов в ...\User\ - Escape - остановка обхода
            FileChecker fileChecker = new FileChecker();
            fileChecker.FileFound += FileChecker_WasFounded;
            fileChecker.WalkDir(new DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        }

        // === === === обработка события
        private static void FileChecker_WasFounded(object sender, MySearchingFileEventArgs e)
        {
            if (e.Handle)
            {
                throw new ArgumentException();
            }
            else
            {
                Console.WriteLine($"Founded: {e.fileInfo.Name}");
            }   
        }
    }

    /// <summary>
    /// поиск наибольшего элемента коллекции
    /// </summary>
    internal static class CollectionGetMax
    {
        public static T GetMax<T>(this IEnumerable<T> collection, Func<T, float> convertToNumber) where T : class
        {
            return collection.OrderByDescending(a => convertToNumber(a)).First();
        }
    }

    /// <summary>
    /// класс с событием нахождения файла
    /// </summary>
    internal class FileChecker
    {
        //public delegate void FileSearchingHandler(object sender, MySearchingFileEventArgs e);
        //public event FileSearchingHandler? FileFound;
        public event EventHandler<MySearchingFileEventArgs> FileFound;

        public void WalkDir(DirectoryInfo root)
        {
            FileInfo[] files = null; DirectoryInfo[] subDirs = null; subDirs = root.GetDirectories();
            foreach (DirectoryInfo dirInfo in subDirs)
            {
                try
                {
                    for (int i = 0; i < dirInfo.GetFiles().Length; i++)
                    {
                        FileFound?.Invoke(this, new MySearchingFileEventArgs(true, dirInfo.GetFiles()[i], (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)));
                        Thread.Sleep(400);
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Приостановлено"); return;
                }
            }
        }
    }

    /// <summary>
    /// кастомизированный EventArgs
    /// </summary>
    internal class MySearchingFileEventArgs : EventArgs
    {
        public bool Handle { get; set; }
        public readonly bool exist;
        public readonly FileInfo fileInfo;
        public MySearchingFileEventArgs(bool _exist, FileInfo _fileInfo, bool _Handle)
        {
            exist = _exist; fileInfo = _fileInfo; Handle = _Handle;
        }
    }
}