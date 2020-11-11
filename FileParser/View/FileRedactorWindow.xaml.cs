using FileParser.Model;
using FileParser.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FileParser
{
    /// <summary>
    /// Interaction logic for FileRedactorWindow.xaml
    /// </summary>
    public partial class FileRedactorWindow : Window
    {
        
        private Logger logger;
        private string[] textArr;
        Dispatcher dispatcher;
        StreamReader sr;
        public FileRedactorWindow(FileInfo file, string word, bool isUniqueWord)
        {
            logger = new Logger();
            InitializeComponent();
            dispatcher = Dispatcher.CurrentDispatcher;
            DataContext = new FileRedactorWindowVM(file);
            Task.Run(() => GetText(file, word, isUniqueWord));

        }
        public void GetText(FileInfo file, string word, bool isUniqueWord)
        {
            using (new PerformanceTimer(logger, $"GetText"))
            {
                using (sr = file.OpenText())
                {
                    dispatcher.Invoke(new Action(() =>
                    {
                        rtbText.Text = sr.ReadToEnd();
                    }));
                }
            }
        }
        private void WriteToRichTextBox(FileInfo file, string word, bool isUniqueWord)
        {
            while (!sr.EndOfStream)
            {
                dispatcher.Invoke(new Action(() =>
                {
                    Paragraph p = new Paragraph();
                    string text = sr.ReadLine();


                    if (text.Contains(word))
                    {
                        textArr = text.Split(' ');
                        foreach (var w in textArr)
                        {
                            string nWord = w + " ";
                            if (isUniqueWord)
                            {
                                if (w == word)
                                {
                                    p.Inlines.Add(new Bold(new Run(nWord) { Foreground = Brushes.Red }));
                                }
                                else
                                {
                                    p.Inlines.Add(new Run(nWord));
                                }
                            }
                            else
                            {
                                if (w.Contains(word))
                                {
                                    p.Inlines.Add(new Bold(new Run(nWord) { Foreground = Brushes.Red }));
                                }
                                else
                                {
                                    p.Inlines.Add(new Run(nWord));
                                }
                            }
                        }
                    }
                    else
                    {
                        p.Inlines.Add(new Run(text));
                    }
                    //rtbText.Document.Blocks.Add(p);
                }));
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (sr != null)
                sr.Dispose();
            GC.Collect();
        }
    }
}