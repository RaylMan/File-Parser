using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParser.Model
{
    public class FilesParser
    {
        private static List<FileInfo> Files = new List<FileInfo>();

        public static List<FileInfo> GetFilesByTree(string path, FileExtension ext)
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
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }


                foreach (string file in files)
                {
                    try
                    {

                        FileInfo fi = new FileInfo(file);
                        if (fi.Extension == ext.Extension)
                            Files.Add(fi);
                        //filesPaths.Add(new FilePathInfo { FileName = fi.Name, FilePath = fi.FullName});
                        //Console.WriteLine("{0}: {1}, {2}", fi.Name, fi.Length, fi.CreationTime);
                    }
                    catch (FileNotFoundException e)
                    {
                        // If file was deleted by a separate application
                        //  or thread since the call to TraverseTree()
                        // then just continue.
                        Console.WriteLine(e.Message);
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
        public static List<FileInfo> GetParseFiles(List<FileInfo> files, string parseText, bool IsUniqueTex)
        {
            List<FileInfo> parseFiles = new List<FileInfo>();
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (IsContaintText(file, parseText, IsUniqueTex))
                    {
                        parseFiles.Add(file);
                    }
                }
            }
            return parseFiles;
        }
        private static bool IsContaintText(FileInfo file, string text, bool IsUniqueText)
        {
            using (StreamReader sr = file.OpenText())
            {
                string fileText = sr.ReadToEnd();

                string[] bufer = fileText?.Split(' ');
                string query = null;
                if (IsUniqueText)
                    query = bufer.Where(w => w == text).FirstOrDefault();
                else
                    query = bufer.Where(w => w.Contains(text)).FirstOrDefault();

                if (query != null)
                {
                    if (fileText.Contains(text))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void AddNewExtension(FileExtension extension)
        {
            if (!IsOriginalExtension(extension.Extension))
                throw new ArgumentException("Расширение уже есть в списке");

            using (StreamWriter sw = new StreamWriter("Extensions.txt", true))
            {
                sw.Write(extension.Extension + " ");
            }
        }
        private static bool IsOriginalExtension(string ext)
        {
            try
            {
                foreach (var item in ReadExtensions())
                {
                    if (item.Extension == ext) return false;

                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public static List<FileExtension> ReadExtensions()
        {
            List<FileExtension> extensions = new List<FileExtension>();
            try
            {
                using (StreamReader sr = new StreamReader("Extensions.txt", true))
                {
                    string text = sr.ReadToEnd();
                    string[] extArr = text.Split(' ');
                    foreach (var ext in extArr)
                    {
                        if (!string.IsNullOrWhiteSpace(ext))
                            extensions.Add(new FileExtension(ext));
                    }
                }
                return extensions;
            }
            catch (System.IO.FileNotFoundException)
            {
                StreamWriter sw = new StreamWriter("Extensions.txt", true);
                sw.Dispose();
                return extensions;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public static void DeleteExtension(FileExtension extension)
        {
            if(extension != null)
            {
                string[] newFile = UpdatedFileExtensionText(extension);
                if(newFile != null)
                {
                    using (StreamWriter sw = new StreamWriter("Extensions.txt", false))
                    {
                        foreach (var item in newFile)
                        {
                            if(!string.IsNullOrWhiteSpace(item))
                                sw.Write(item + " ");
                        }
                    }
                }

            }
        }
        private static string[] UpdatedFileExtensionText(FileExtension extension)
        {
            using (StreamReader sr = new StreamReader("Extensions.txt", true))
            {
                string text = sr.ReadToEnd();
                string[] extArr = text.Split(' ');
                for (int i = 0; i < extArr.Length; i++)
                {
                    if (extArr[i] == extension.Extension)
                    {
                        extArr[i] = null;
                        break;
                    }
                }
                return extArr;
            }
        }
        public static string GetTextFromFile(FileInfo file)
        {
            if (file != null)
            {
                try
                {
                    using (StreamReader sr = file.OpenText())
                    {
                        return sr.ReadToEnd();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return null;
        }
    }
}
