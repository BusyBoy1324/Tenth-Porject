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
        public string Path { get; set; }
        public long Size { get; set; }
    }
}
