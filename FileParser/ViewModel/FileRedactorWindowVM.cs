using FileParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace FileParser.ViewModel
{
    public class FileRedactorWindowVM : ViewModelBase
    {
        FileInfo file;
        private string fileText;
        public string FileText
        {
            get { return fileText; }
            set
            {
                fileText = value;
                RaisedPropertyChanged("FileText");
            }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisedPropertyChanged("Title");
            }
        }
        private FlowDocument document;
        public FlowDocument Document
        {
            get { return document; }

            set
            {
                document = value;
                RaisedPropertyChanged("Document");
            }
        }

        public FileRedactorWindowVM(FileInfo file)
        {
            this.file = file;
            Title = file.FullName;
            //FileText = FilesParser.GetTextFromFile(file);
            //Document = GenarateDocument();
        }

        private FlowDocument GenarateDocument()
        {
            FlowDocument doc = new FlowDocument();
            var p = new Paragraph(new Run(FileText));
            p.FontSize = 16;
            p.FontStyle = FontStyles.Italic;
            doc.Blocks.Add(p);
            return doc;
        }
    }
}
