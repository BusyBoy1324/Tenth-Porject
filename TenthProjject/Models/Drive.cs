using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenthProject.Models
{
    public class Drive : IFileSystemObject
    {
        public Drive(string name, long size = 0, List<IFileSystemObject> childrens = null)
        {
            Name = name;
            Size = (ulong)size;
            NestedObjects = childrens ?? new List<IFileSystemObject>();
        }
        public string Name { get; set; }
        public List<IFileSystemObject> NestedObjects { get; set; }
        public ulong Subdirectories { get; set; }
        public ulong Size { get; set; }
        public ulong Files { get; set; }
    }
}
