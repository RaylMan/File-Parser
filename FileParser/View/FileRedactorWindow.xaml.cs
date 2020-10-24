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

namespace FileParser
{
    /// <summary>
    /// Interaction logic for FileRedactorWindow.xaml
    /// </summary>
    public partial class FileRedactorWindow : Window
    {
        private string[] textArr;
        public FileRedactorWindow(FileInfo file, string word, bool isUniqueWord)
        {
            InitializeComponent();
            DataContext = new FileRedactorWindowVM(file);
            GetText(file, word, isUniqueWord);
        }
        public void GetText(FileInfo file, string word, bool isUniqueWord)
        {
            using (StreamReader sr = file.OpenText())
            {
                while (!sr.EndOfStream)
                {
                    Paragraph p = new Paragraph();
                    string text = sr.ReadLine();
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
                        p.Inlines.Add(new Run(text));
                    }
                    rtbText.Document.Blocks.Add(p);
                }
            }
        }
    }
}
