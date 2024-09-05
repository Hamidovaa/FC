using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Interfaces;
using FC.Models;
using System.IO;



namespace FC.Loaders
{
    public class CsvFileLoader : IFileLoader
    {
        public IEnumerable<FileModel> Load(string filePath)
        {
            List<FileModel> fileModels = new List<FileModel>();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fileModel = new FileModel
                    {
                        FileName = Path.GetFileName(filePath),
                        Data = line // CSV satırını Data olaraq saxlayırıq
                    };
                    fileModels.Add(fileModel);
                }
            }

            return fileModels;
        }
    }

}
