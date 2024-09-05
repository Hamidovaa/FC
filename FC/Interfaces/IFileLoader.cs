using FC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Interfaces
{
    public interface IFileLoader
    {
        IEnumerable<FileModel> Load(string filePath);
    }
}
