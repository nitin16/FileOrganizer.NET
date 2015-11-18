using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileOrganizer
{
    public class FileUtils
    {
        string sourceDir
        {
            get;
            set;
        }

        string destDir
        {
            get;
            set;
        }
        
        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private Regex r = new Regex(":");

        public FileUtils(string sourceDir, string destDir)
        {
            this.sourceDir = sourceDir;
            this.destDir = destDir;
        }

        public IEnumerable<Task> ProcessDir(string dirpath)
        {
            Console.WriteLine("Processecing Dir: " + dirpath);

            var fileNames= from filename in Directory.EnumerateFiles(dirpath, "*.*", SearchOption.AllDirectories)
                            select filename;

            return fileNames.AsParallel().Select(file => ProcessFile(file));//.SelectMany(t=> t);
            //var tasks =  subDirectories.AsParallel().Select(subDirPath => ProcessDir(subDirPath)).SelectMany(t=> t);
            
            //IEnumerable<Task> processFileTasks = dir.GetFiles().ToList().Select(file => ProcessFile(file));
            //await Task.WhenAll(processDirTasks);
            //await Task.WhenAll(processFileTasks);

            //foreach (var subDir in dir.GetDirectories())
            //{
            //    Console.WriteLine("Processecing Dir: " + subDir.FullName);
            //    ProcessDir(subDir);
            //}

            //foreach (var file in dir.GetFiles())
            //{
            //    ProcessFile(file);
            //}
        }

        private async Task ProcessFile(string filePath)
        {
            var file = new FileInfo(filePath);
            if (file == null)
            {
                return;
            }
            //Console.WriteLine("Processecing file : " + file.FullName);

            var targetDir = destDir + "/" + GetTargetDir(file.FullName).Result;
            Directory.CreateDirectory(targetDir);

            file.CopyTo(targetDir + file.Name, true);
            //Console.WriteLine("Copied sucessesfully: " + targetDir + file.Name);
        }


        //retrieves the datetime WITHOUT loading the whole image
        public Task<DateTime> GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return Task.FromResult(DateTime.Parse(dateTaken));
            }
        }

        public async Task<string> GetTargetDir(string FilePath)
        {
            try
            {
                var date = await GetDateTakenFromImage(FilePath);
                return date.ToString("yyyy/MM/");
            }
            catch
            {
                return "/Exceptions/";
            }
        }

        public void processDirSync(string dirpath)
        {
            var dir = new DirectoryInfo(dirpath);
            foreach (var subDir in dir.GetDirectories())
            {
                Console.WriteLine("Processecing Dir: " + subDir.FullName);
                processDirSync(subDir.FullName);
            }

            foreach (var file in dir.GetFiles())
            {
                ProcessFile(file.FullName);
            }
        }
    }
}
