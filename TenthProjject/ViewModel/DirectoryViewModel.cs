using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenthProject.ViewModels;

namespace TenthProject.ViewModel
{
    public class DirectoryViewModel : TreeViewModel
    {
        public DirectoryViewModel(Models.Directory directory, TreeViewModel parentDrive)
            : base(parentDrive, true)
        {
            _fileSystemObject = directory;
        }

        protected override void LoadChildren()
        {
            foreach (string directory in System.IO.Directory.GetDirectories(_fileSystemObject.Path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                base.Children.Add(new DirectoryViewModel(new Models.Directory(directoryInfo.Name, directory), this));
            }
            foreach (string file in System.IO.Directory.GetFiles(_fileSystemObject.Path))
            {
                FileInfo fileInfo = new FileInfo(file);
                base.Children.Add(new FileViewModel(new Models.File(fileInfo), this));
            }
        }
    }
}
