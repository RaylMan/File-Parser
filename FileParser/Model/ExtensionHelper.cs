using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParser.Model
{
    public static class ExtensionHelper
    {

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
            var oldExt = ReadExtensions().FirstOrDefault(e => e.Extension == ext);

            if (oldExt != null)
                return false;
            else
                return true;
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
            if (extension != null)
            {
                string[] newFile = UpdatedFileExtensionText(extension);
                if (newFile != null)
                {
                    using (StreamWriter sw = new StreamWriter("Extensions.txt", false))
                    {
                        foreach (var item in newFile)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
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
    }
}
