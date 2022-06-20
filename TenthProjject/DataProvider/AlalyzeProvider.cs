using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveAnalyzer;
using Models;

namespace TenthProject.DataProvider
{
    public class AlalyzeProvider
    {
        public IFileSystemDirectory ScanDirectory(string path)
        {
            return DriveAnalyzer.DriveAnalyzer.ScanDirectory(path).Result;
        }
    }
}
