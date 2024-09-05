using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Interfaces;
using FC.Models;
using System.Xml.Linq;
using System.IO;


namespace FC.Loaders
{
    public class XmlFileLoader : IFileLoader
    {
        public IEnumerable<FileModel> Load(string filePath)
        {
            List<FileModel> fileModels = new List<FileModel>();

            XDocument xmlDocument = XDocument.Load(filePath);
            foreach (var element in xmlDocument.Descendants("Item"))
            {
                var fileModel = new FileModel
                {
                    FileName = Path.GetFileName(filePath),
                    Data = element.Value
                };
                fileModels.Add(fileModel);
            }

            return fileModels;
        }
    }

}
