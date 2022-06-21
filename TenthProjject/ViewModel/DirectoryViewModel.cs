using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenthProject.ViewModel
{
    public class DirectoryViewModel :TreeViewModel
    {
        public DirectoryViewModel(Models.Directory directory, TreeViewModel parentDrive)
            : base(parentDrive, true)
        {
            FileSystemObject = directory;
        }

        protected override void LoadChildren()
        {
            foreach (string directory in System.IO.Directory.GetDirectories(FileSystemObject.Path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                base.Children.Add(new DirectoryViewModel(new Models.Directory(directoryInfo.Name, directory, FileSystemObject.Size), this));
            }
            foreach (string file in System.IO.Directory.GetFiles(FileSystemObject.Path))
            {
                FileInfo fileInfo = new FileInfo(file);
                base.Children.Add(new FileViewModel(new Models.File(fileInfo.Name, FileSystemObject.Size), this));
            }
        }
    }
}
