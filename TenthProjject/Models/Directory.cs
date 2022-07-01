using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenthProject.Models
{
    public class Directory : IFileSystemObject
    {
        public Directory(string name, long size = 0, ObservableCollection<IFileSystemObject> childrens = null)
        {
            Name = name;
            Size = (ulong)size;
            NestedObjects = childrens ?? new ObservableCollection<IFileSystemObject>();
        }
        public string Name { get; set; }
        public ulong Size { get; set; }
        public ulong Files { get; set; }
        public ObservableCollection<IFileSystemObject> NestedObjects { get; set; }
    }
}
