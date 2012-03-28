using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MongoRunner
{
    class Program
    {
        private const string dataDirectory = ".\\data";
        private const string dbDirectory = "\\db";
        private static string dataDbDirectory = string.Format("{0}{1}", dataDirectory, dbDirectory);
        private static DirectoryInfo mongoPath;

        static void Main()
        {
            Ensure();
            var psi = CreateProcessStartInfo();

            var process = Process.Start(psi);


            if (process.HasExited == false)
            {
                Console.WriteLine("Press return to stop MongoDB from running:");
                Console.ReadLine();

                process.CloseMainWindow();
                process.WaitForExit(1000 * 5);

                if (process.HasExited == false)
                {
                    process.Kill();
                }
            }
        }

        private static void Ensure()
        {
            var current = Directory.GetCurrentDirectory();

            try
            {
                var path = GetMongoPath();

                if (path != null)
                {
                    Directory.SetCurrentDirectory(path.FullName);

                    if (Directory.Exists(dataDbDirectory) == false)
                    {
                        Directory.CreateDirectory(dataDbDirectory);
                    }
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(current);
            }
        }

        private static ProcessStartInfo CreateProcessStartInfo()
        {

            var path = GetMongoPath();

            if (path != null)
            {
                var mongod = path.GetFiles("mongod.exe").FirstOrDefault();

                if (mongod != null)
                {
                    return new ProcessStartInfo
                               {
                                   FileName = mongod.FullName,
                                   WorkingDirectory = path.FullName,
                                   Arguments = String.Format("--dbpath {0} --port 9999 --rest --logpath {0}\\output.log", dataDbDirectory)
                               };
                }
            }

            throw new ArgumentException("Missing Mongo Server");
        }

        private static DirectoryInfo GetMongoPath()
        {
            if (mongoPath == null)
            {
                var path = new DirectoryInfo(Directory.GetCurrentDirectory());

                if (path.Parent != null && path.Parent.Parent != null && path.Parent.Parent.Parent != null)
                {
                    mongoPath = path.Parent.Parent.Parent.GetDirectories("mongo").FirstOrDefault();
                }    
            }

            return mongoPath;
        }
    }
}
