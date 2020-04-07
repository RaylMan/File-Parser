using FileParser.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace FileParser.ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        private int filesCount;
        private readonly Dispatcher dispatcher;
        private ObservableCollection<FileExtension> extensions = new ObservableCollection<FileExtension>();
        public ObservableCollection<FileExtension> Extensions
        {
            get { return extensions; }
            set
            {
                extensions = value;
                RaisedPropertyChanged("Extensions");
            }
        }
        private ObservableCollection<FileInfo> files = new ObservableCollection<FileInfo>();
        public ObservableCollection<FileInfo> Files
        {
            get { return files; }
            set
            {
                files = value;
                RaisedPropertyChanged("Files");
            }
        }
        private FileInfo selectedFile;
        public FileInfo SelectedFile
        {
            get { return selectedFile; }
            set
            {
                selectedFile = value;
                RaisedPropertyChanged("SelectedFile");
            }
        }
        private FileExtension selectedExtension;
        public FileExtension SelectedExtension
        {
            get { return selectedExtension; }
            set
            {
                selectedExtension = value;
                RaisedPropertyChanged("SelectedExtension");
            }
        }


        private string extensionText;
        public string ExtensionText
        {
            get { return extensionText; }
            set
            {
                extensionText = value;
                RaisedPropertyChanged("ExtensionText");
            }
        }
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                RaisedPropertyChanged("FilePath");
            }
        }

        private string searchWord;
        public string SearchWord
        {
            get { return searchWord; }
            set
            {
                searchWord = value;
                RaisedPropertyChanged("SearchWord");
            }
        }
        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                RaisedPropertyChanged("Status");
            }
        }
        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                RaisedPropertyChanged("IsEnabled");
            }
        }
        private bool isUniqueWord;
        public bool IsUniqueWord
        {
            get { return isUniqueWord; }
            set
            {
                isUniqueWord = value;
                RaisedPropertyChanged("IsUniqueWord");
            }
        }

        public MainWindowVM()
        {
            IsEnabled = true;
            dispatcher = Dispatcher.CurrentDispatcher;
            FillExtensions();
            ExtensionText = ".";
           // filePath = @"D:\bootstrap-4.1.3";
        }

        private void FillExtensions()
        {
            Extensions.Clear();
            Extensions = new ObservableCollection<FileExtension>(FilesParser.ReadExtensions());
        }
        #region Save New Extension
        private ICommand saveExtensionCommand;
        public ICommand SaveExtensionCommand
        {
            get
            {
                if (saveExtensionCommand == null)
                {
                    saveExtensionCommand = new RelayCommand(new Action<object>(SaveNewExtension));
                }
                return saveExtensionCommand;
            }
            set
            {
                saveExtensionCommand = value;
                RaisedPropertyChanged("SaveExtensionCommand");
            }
        }

        private void SaveNewExtension(object obj)
        {
            if (!string.IsNullOrWhiteSpace(ExtensionText))
            {
                FileExtension extension = new FileExtension(ExtensionText);
                try
                {
                    FilesParser.AddNewExtension(extension);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                ExtensionText = ".";
                FillExtensions();
            }
        }
        #endregion

        #region ParseFilesCommand
        private ICommand parseFilesCommand;
        public ICommand ParseFilesCommand
        {
            get
            {
                if (parseFilesCommand == null)
                {
                    parseFilesCommand = new RelayCommand(new Action<object>(ParseFilesAsync));
                }
                return parseFilesCommand;
            }
            set
            {
                parseFilesCommand = value;
                RaisedPropertyChanged("ParseFilesCommand");
            }
        }
        private async void ParseFilesAsync(object obj)
        {
            IsEnabled = false;
            Status = "Выполнение операции!";
            Files.Clear();
            await Task.Run(() => ParseFiles(new object()));
            IsEnabled = true;
            Status = $"Операция завершена! Найдено в: {filesCount} файлах";
        }
        private void ParseFiles(object obj)
        {
            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                if (!string.IsNullOrWhiteSpace(SearchWord))
                {
                    filesCount = 0;
                    foreach (var ext in Extensions)
                    {
                        if (ext.IsChecked)
                        {
                            var filesByExtension = FilesParser.GetFilesByTree(FilePath, ext);
                            if (filesByExtension != null)
                            {
                                foreach (var file in FilesParser.GetParseFiles(filesByExtension, SearchWord, IsUniqueWord))
                                {
                                    filesCount++;
                                    dispatcher.Invoke(new Action(() =>
                                    {
                                        Status = $"Выполнение операции! Найдено в {filesCount} файлах";
                                        Files.Add(file);
                                    }));
                                }
                            }
                        }
                    }
                }
                else
                    MessageBox.Show("Введите слово для поиска!", "Ошибка");
                
            }
            else
            {
                MessageBox.Show("Укажите папку для поиска!", "Ошибка");
            }
        }
        #endregion

        #region SelectedFileInfoCommand
        private ICommand selectedFileInfoCommand;
        public ICommand SelectedFileInfoCommand
        {
            get
            {
                if (selectedFileInfoCommand == null)
                {
                    selectedFileInfoCommand = new RelayCommand(new Action<object>(SelectedFileInfo));
                }
                return selectedFileInfoCommand;
            }
            set
            {
                selectedFileInfoCommand = value;
                RaisedPropertyChanged("SelectedFileInfoCommand");
            }
        }

        private void SelectedFileInfo(object obj)
        {
            if(SelectedFile != null)
            {
                string info = $"Назание файла:  {SelectedFile.Name}\n" +
                    $"Путь: {SelectedFile.FullName}\n";
                MessageBox.Show(info,"Информация");
            }
        }
        #endregion

        #region DeleteExtensionCommand
        private ICommand deleteExtensionCommand;
        public ICommand DeleteExtensionCommand
        {
            get
            {
                if (deleteExtensionCommand == null)
                {
                    deleteExtensionCommand = new RelayCommand(new Action<object>(DeleteExtension));
                }
                return deleteExtensionCommand;
            }
            set
            {
                deleteExtensionCommand = value;
                RaisedPropertyChanged("DeleteExtensionCommand");
            }
        }

        private void DeleteExtension(object obj)
        {
            if (SelectedExtension != null)
            {
                var result = MessageBox.Show("Вы действительно хотете удалить?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        FilesParser.DeleteExtension(SelectedExtension);
                        FillExtensions();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }
        #endregion

        #region OpenPathCommand
        private ICommand openPathCommand;
        public ICommand OpenPathCommand
        {
            get
            {
                if (openPathCommand == null)
                {
                    openPathCommand = new RelayCommand(new Action<object>(OpenPath));
                }
                return openPathCommand;
            }
            set
            {
                openPathCommand = value;
                RaisedPropertyChanged("OpenPathCommand");
            }
        }

        private void OpenPath(object obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    FilePath = fbd.SelectedPath;
                }
            }
        }
        #endregion
    }
}
