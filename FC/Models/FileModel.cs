﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FilePath { get; set; }
        public string Data { get; set; }

        public override string ToString()
        {
            return FileName; 
        }
    }

}
