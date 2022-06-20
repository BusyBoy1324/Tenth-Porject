﻿using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenthProject.ViewModels;

namespace TenthProject.ViewModel
{
    public class FileViewModel : TreeViewModel
    {
        public FileViewModel(File file, TreeViewModel parentDirectory)
            : base(parentDirectory, false)
        {
            _fileSystemObject = file;
        }

    }
}
