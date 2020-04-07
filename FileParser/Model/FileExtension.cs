using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParser.Model
{
   
    public class FileExtension
    {
        public string Extension { get; set; }
        public bool IsChecked { get; set; }
        public FileExtension(string extension)
        {
            Extension = extension;
        }
    }
}
