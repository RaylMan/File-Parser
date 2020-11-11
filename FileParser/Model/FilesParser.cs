using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileParser.Model
{
    public class FilesParser
    {
        private Logger logger;
        private static SemaphoreSlim sem = new SemaphoreSlim(6, 6);
        private List<FileInfo> Files = new List<FileInfo>();

        public FilesParser(Logger logger)
        {
            this.logger = logger;
        }
        public List<FileInfo> GetFilesByTree(string path, FileExtension ext)
        {
            Files.Clear();
            Stack<string> dirs = new Stack<string>(20);

            if (!Directory.Exists(path))
            {
                throw new ArgumentException();
            }
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {
                    logger.LogMessage(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    logger.LogMessage(e.Message);
                    continue;
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    logger.LogMessage(e.Message);
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    logger.LogMessage(e.Message);
                    continue;
                }


                foreach (string file in files)
                {
                    try
                    {

                        FileInfo fi = new FileInfo(file);
                        if (fi.Extension == ext.Extension)
                            Files.Add(fi);
                    }
                    catch (FileNotFoundException e)
                    {
                        // If file was deleted by a separate application
                        //  or thread since the call to TraverseTree()
                        // then just continue.
                        logger.LogMessage(e.Message);
                        continue;
                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (string str in subDirs)
                    dirs.Push(str);
            }
            return Files;
        }

        public bool IsContaintText(FileInfo file, string text, bool IsUniqueText, CancellationToken token)
        {
            if (file.Length > 10000)
                sem.Wait();

            using (StreamReader sr = file.OpenText())
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (token.IsCancellationRequested)
                    {
                        sr.Dispose();
                        sem.Release();
                        token.ThrowIfCancellationRequested();
                    }

                    string[] bufer = line?.Split(' ');
                    string query = null;
                    if (IsUniqueText)
                        query = bufer.FirstOrDefault(w => string.Equals(w, text, StringComparison.OrdinalIgnoreCase));
                    else
                        query = bufer.FirstOrDefault(w => w.Contains(text));

                    if (query != null)
                    {
                        if (file.Length > 10000)
                            sem.Release();
                        return true;
                    }
                }
            }
            if (file.Length > 10000)
                sem.Release();
            return false;
        }

        public static string GetTextFromFile(FileInfo file)
        {
            if (file != null)
            {
                using (StreamReader sr = file.OpenText())
                {
                    return sr.ReadToEnd();
                }
            }
            return "File not found";
        }
    }
}
