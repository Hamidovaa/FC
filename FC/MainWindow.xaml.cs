using FC.Context;
using FC.Models;
using Microsoft.Extensions.Configuration;
using System.Configuration;
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


namespace FC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher _fileSystemWatcher;
        private IConfiguration _configuration;
        private MonitoringSettings _monitoringSettings;

        public MainWindow()
        {
            InitializeComponent();

            // Configuration setup
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the application directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); // Add JSON configuration file
            _configuration = builder.Build();

            // Check if the settings are loaded correctly
            var pollingInterval = _configuration["Monitoring:PollingInterval"];
            if (pollingInterval == null)
            {
                MessageBox.Show("PollingInterval konfiqurasiya faylında tapılmadı.");
            }
            else
            {
                // Bind the Monitoring section to MonitoringSettings model
                _monitoringSettings = _configuration.GetSection("Monitoring").Get<MonitoringSettings>();

                // Initialize the file system watcher and start monitoring
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
            // Konfiqurasiya dəyərini oxumaq
            string pollingIntervalString = _configuration["Monitoring:PollingInterval"];
            if (int.TryParse(pollingIntervalString, out int pollingInterval))
            {
                // Wait for the specified interval before processing
                await Task.Delay(pollingInterval);

                // Faylın adını ListBox-a əlavə etmək
                Dispatcher.Invoke(() =>
                {
                    FilesListBox.Items.Add(e.Name);
                });

                try
                {
                    using (var context = new AppDbContext())
                    {
                        var fileModel = new FileModel
                        {
                            FileName = e.Name,
                            CreatedDate = DateTime.Now
                        };

                        context.Files.Add(fileModel);
                        await context.SaveChangesAsync(); // Asenkron SaveChanges
                    }
                }
                catch (Exception ex)
                {
                    // Hata işleme (Mesela bir log dosyasına yazmak veya kullanıcıya bildirim yapmak)
                    MessageBox.Show($"Veritabanı hatası: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("PollingInterval konfiqurasiya dəyəri düzgün deyil.");
            }
        }


    }
}

    
