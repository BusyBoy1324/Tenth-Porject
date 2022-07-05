using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenthProject.Models
{
    public interface IFileSystemObject
    {
        public string Name { get; set; }
        //public UInt64 Size { get; set; }
        public double Size { get; set; }
        public UInt64 Files { get; set; }
    }
}
