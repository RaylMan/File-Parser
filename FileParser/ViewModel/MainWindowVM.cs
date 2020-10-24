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
        #region Properties and Constructor
        private int allfilesCount = 0;
        private Logger logger;
        private FilesParser filesParser;
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
            logger = new Logger();
            filesParser = new FilesParser(logger);

            IsEnabled = true;
            dispatcher = Dispatcher.CurrentDispatcher;
            FillExtensions();
            ExtensionText = ".";

            //test
            filePath = @"D:\test parser";
            searchWord = "Hello";
            var ext = extensions.FirstOrDefault(e => e.Extension.Contains("txt"));
            ext.IsChecked = true;
        }

        private void FillExtensions()
        {
            Extensions.Clear();
            Extensions = new ObservableCollection<FileExtension>(ExtensionHelper.ReadExtensions());
        }
        #endregion

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
                    ExtensionHelper.AddNewExtension(extension);
                }
                catch (Exception ex)
                {
                    logger.LogMessage(ex.Message);
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
            try
            {
                IsEnabled = false;
                Status = "Выполнение операции!";
                Files.Clear();
                await Task.Run(ParseFiles);
                IsEnabled = true;
                Status = $"Операция завершена! Найдено в: {filesCount} из {allfilesCount}";
                logger.LogBuilder();
            }
            catch (Exception e)
            {
                logger.LogMessage(e.Message);
                MessageBox.Show(e.Message, "Ошибка");
            }

        }

        private async Task ParseFiles()
        {
            using (new PerformanceTimer(logger, $"ParseFiles"))
            {
                if (string.IsNullOrWhiteSpace(FilePath))
                {
                    MessageBox.Show("Укажите папку для поиска!", "Ошибка");
                    return;
                }
                if (string.IsNullOrWhiteSpace(SearchWord))
                {
                    MessageBox.Show("Введите слово для поиска!", "Ошибка");
                    return;
                }

                filesCount = 0;

                var checkedExt = Extensions.Where(e => e.IsChecked).ToList();
                if (checkedExt != null && checkedExt.Count > 0)
                {

                    foreach (var ext in checkedExt)
                    {
                        var filesByExtension = filesParser.GetFilesByTree(FilePath, ext);

                        if (filesByExtension != null)
                        {
                            allfilesCount += filesByExtension.Count;
                            List<Task> parseTasks = new List<Task>();

                            foreach (var file in filesByExtension)
                            {
                                parseTasks.Add(Task.Run(() => SearchInFile(file)));
                            }
                            await Task.WhenAll(parseTasks);
                        }
                    }
                }
            }
        }

        private void SearchInFile(FileInfo file)
        {
            if (filesParser.IsContaintText(file, searchWord, isUniqueWord))
            {
                filesCount++;
                dispatcher.Invoke(new Action(() =>
                {
                    Status = $"Выполнение операции! Найдено в {filesCount} из {allfilesCount}";
                    Files.Add(file);
                }));
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
            if (SelectedFile != null)
            {
                string info = $"Назание файла:  {SelectedFile.Name}\n" +
                    $"Путь: {SelectedFile.FullName }\nРазмер: { SelectedFile.Length / 1024} Кб\n";
                MessageBox.Show(info, "Информация");
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
                        ExtensionHelper.DeleteExtension(SelectedExtension);
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
