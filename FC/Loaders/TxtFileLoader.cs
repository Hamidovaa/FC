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
    public class TxtFileLoader : IFileLoader
    {
        public IEnumerable<FileModel> Load(string filePath)
        {
            List<FileModel> fileModels = new List<FileModel>();

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var fileModel = new FileModel
                {
                    FileName = Path.GetFileName(filePath),
                    Data = line
                };
                fileModels.Add(fileModel);
            }

            return fileModels;
        }
    }

}
