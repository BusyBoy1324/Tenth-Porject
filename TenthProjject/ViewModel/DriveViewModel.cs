using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenthProject.ViewModel
{
    public class DriveViewModel : TreeViewModel
    {
        public DriveViewModel(Drive drive)
            : base(null, true)
        {
            FileSystemObject = drive;
        }
        protected override void LoadChildren()
        {
            foreach (string directory in System.IO.Directory.GetDirectories(FileSystemObject.Name))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                base.Children.Add(new DirectoryViewModel(new Models.Directory(directoryInfo.Name, directory), this));
            }
            foreach (string file in System.IO.Directory.GetFiles(FileSystemObject.Name))
            {
                FileInfo fileInfo = new FileInfo(file);
                base.Children.Add(new FileViewModel(new Models.File(fileInfo), this));
            }
        }
    }
}
