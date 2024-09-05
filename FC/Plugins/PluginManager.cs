using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace FC.Plugins
{

    public class PluginManager
    {
        private List<IFileLoader> _fileLoaders = new List<IFileLoader>();

        public PluginManager(string pluginFolderPath)
        {
            LoadPlugins(pluginFolderPath);
        }

        private void LoadPlugins(string pluginFolderPath)
        {
            if (Directory.Exists(pluginFolderPath))
            {
                var pluginFiles = Directory.GetFiles(pluginFolderPath, "*.dll");

                foreach (var pluginFile in pluginFiles)
                {
                    Assembly assembly = Assembly.LoadFrom(pluginFile);

                    var loaders = assembly.GetTypes()
                        .Where(t => typeof(IFileLoader).IsAssignableFrom(t) && !t.IsInterface)
                        .Select(t => (IFileLoader)Activator.CreateInstance(t));

                    _fileLoaders.AddRange(loaders);
                }
            }
        }

        public IFileLoader GetFileLoader(string fileExtension)
        {
            return _fileLoaders.FirstOrDefault(l => l.GetType().Name.ToLower().Contains(fileExtension.ToLower()));
        }
    }

    public class FileProcessingService
    {
        private PluginManager _pluginManager;

        public FileProcessingService(string pluginPath)
        {
            _pluginManager = new PluginManager(pluginPath);
        }

        public void ProcessFile(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower().Replace(".", "");
            var loader = _pluginManager.GetFileLoader(fileExtension);

            if (loader != null)
            {
                var fileModels = loader.Load(filePath);
                foreach (var fileModel in fileModels)
                {
                    Console.WriteLine($"İçerik: {fileModel.FileName}");
                }
            }
            else
            {
                Console.WriteLine("Bu dosya formatı desteklenmiyor.");
            }
        }
    }


}
