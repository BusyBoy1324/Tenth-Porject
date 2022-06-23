using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Drive : IFileSystemDirectory
    {
        public Drive(string name)
        {
            this.Name = name;
            this.Childrens = new ObservableCollection<IFileSystemObject>();
        }
        public string Name { get; set; }
        public ObservableCollection<IFileSystemObject> Childrens { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
    }
}
