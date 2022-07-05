using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenthProject.Models
{
    public class File : IFileSystemObject
    {
        private string _name;
        //private ulong _size;
        private double _size;
        private string _path;
        private ulong _files;

        public string Name
        {
            get => _name;
            set { _name = value; }
        }
        //public ulong Size
        //{
        //    get => _size;
        //    set { _size = value; }
        //}
        public double Size
        {
            get => _size;
            set { _size = value; }
        }

        public string Path
        {
            get => _path;
            set { _path = value; }
        }

        public ulong Files
        {
            get => _files;
            set { _files = value; }
        }
        public File(System.IO.FileInfo fileInfo)
        {
            _name = fileInfo.Name;
            _size = (ulong)fileInfo.Length;
            _path = fileInfo.FullName;
        }
    }
}
