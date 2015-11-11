using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace FileOrganizer
{
    public class Program
    {
        static string sourceDir;
        static string destDir;
        public static void Main(string[] args)
        {
            if (string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine("Missing Source or destination directories");
            }
             sourceDir = args[0];
             destDir = args[1];

            Console.WriteLine("Processecing Dir: " + sourceDir);
            var dir = new DirectoryInfo(sourceDir);
            ProcessDir(dir);
            Console.ReadLine();
        }

        private static void ProcessDir(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                Console.WriteLine("Processecing Dir: " + subDir.FullName);
                ProcessDir(subDir);
            }

            foreach (var file in dir.GetFiles())
            {
                ProcessFile(file);
            }
        }

        private static void ProcessFile(FileInfo file)
        {
            return;

            if (file == null)
            {
                return;
            }
            Console.WriteLine("Processecing file : " + file.FullName);

            var targetDir = destDir + "/" + GetTargetDir(file.FullName);
            Directory.CreateDirectory(targetDir);
            
            file.CopyTo(targetDir + file.Name, true);
            Console.WriteLine("Copied sucessesfully: " + targetDir + file.Name);
        }

        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        public static string GetTargetDir(string FilePath)
        {
            try
            {
                return GetDateTakenFromImage(FilePath).ToString("yyyy/MM/");
            }
            catch
            {
                return "/Exceptions/";
            }
        }
    }
}
