using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace FileOrganizer
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            if (string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine("Missing Source or destination directories");
            }

            var sourceDir = args[0];
            var destDir = args[1];
            FileUtils helper = new FileUtils(sourceDir, destDir);

            var startTime = DateTime.Now;
            Task.WaitAll(helper.ProcessDir(sourceDir).ToArray());
            //helper.processDirSync(sourceDir);
            Console.WriteLine("Processing Complete");
            Console.WriteLine(String.Format("Total time: {0}", DateTime.Now - startTime));
            
            Console.ReadLine();
        }

    }
}
