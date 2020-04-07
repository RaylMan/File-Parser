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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string parseWord;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM();
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(Files.SelectedItem != null)
            {
                FileRedactorWindow win = new FileRedactorWindow((FileInfo)Files.SelectedItem, parseWord, chbxUnique.IsChecked.Value);
                win.Show();
            }
           
        }

        private void Parse_Click(object sender, RoutedEventArgs e)
        {
            parseWord = tbxSearchWord.Text;
        }
    }
}
