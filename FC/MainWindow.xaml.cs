using FC.Context;
using FC.Interfaces;
using FC.Loaders;
using FC.Models;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;


namespace FC
{
   
    public partial class MainWindow : Window
    {
        private FileSystemWatcher _fileSystemWatcher;
        private IConfiguration _configuration;
        private MonitoringSettings _monitoringSettings;

        public MainWindow()
        {
            InitializeComponent();

            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); 
            _configuration = builder.Build();

           
            var pollingInterval = _configuration["Monitoring:PollingInterval"];
            if (pollingInterval == null)
            {
                MessageBox.Show("PollingInterval konfiqurasiya faylında tapılmadı.");
            }
            else
            {
                
                _monitoringSettings = _configuration.GetSection("Monitoring").Get<MonitoringSettings>();

                
                InitializeFileSystemWatcher();
            }
        }

        private void InitializeFileSystemWatcher()
        {
            string folderPath = _monitoringSettings.DirectoryPath;
            int monitoringInterval = _monitoringSettings.PollingInterval;

            if (Directory.Exists(folderPath))
            {
                _fileSystemWatcher = new FileSystemWatcher(folderPath)
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                    Filter = "*.*",
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true
                };

                _fileSystemWatcher.Created += OnNewFileCreated;

                MessageBox.Show("Monitoring started.");
            }
            else
            {
                MessageBox.Show("Folder path does not exist.");
            }
        }



        private async void OnNewFileCreated(object sender, FileSystemEventArgs e)
        {
            string fileExtension = Path.GetExtension(e.FullPath);

            try
            {
                IFileLoader fileLoader = FileLoaderFactory.GetLoader(fileExtension);
                var fileModels = fileLoader.Load(e.FullPath);

                Dispatcher.Invoke(() =>
                {
                    foreach (var fileModel in fileModels)
                    {
                        fileModel.FilePath = e.FullPath;


                        if (FilesListBox.Items.OfType<FileModel>().All(f => f.FilePath != fileModel.FilePath.ToString()))
                        {
                            FilesListBox.Items.Add(fileModel);
                        }
                    }
                });

               
                using (var context = new AppDbContext())
                {
                    context.Files.AddRange(fileModels);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}");
            }
        }


        private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesListBox.SelectedItem is FileModel selectedFileModel)
            {
                string filePath = selectedFileModel.FilePath;

                if (File.Exists(filePath))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("File does not exist.");
                }
            }
        }



    }
}

    
