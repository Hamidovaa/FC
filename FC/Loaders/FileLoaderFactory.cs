using FC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Loaders
{
    public class FileLoaderFactory
    {
        public static IFileLoader GetLoader(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".xml":
                    return new XmlFileLoader();
                case ".csv":
                    return new CsvFileLoader();
                case ".txt":
                    return new TxtFileLoader();
                default:
                    throw new NotSupportedException($"Unsupported file type: {fileExtension}");
            }
        }
    }

}
