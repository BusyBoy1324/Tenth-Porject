using AutoMapper;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TenthProject.DataProvider;
using TenthProject.Models;

namespace TenthProject.ViewModel
{
    public enum DisplayedUnit
    {
        Kilobyte,
        Megabyte,
        Gigabyte
    }
    public class MainWindowViewModel
    {
        public IFileSystemObject _fsObject;
        public ObservableCollection<Drive> Drives { get; set; }
        public string Name;
        public long Size;
        public string Path;
        public static long size;
        readonly AlalyzeProvider dataProvider = new AlalyzeProvider();
        public MainWindowViewModel()
        {
            KB = new DelegateCommand.DelegateCommand(OnClick_KB);
            MB = new DelegateCommand.DelegateCommand(OnClick_MB);
            GB = new DelegateCommand.DelegateCommand(OnClick_GB);
            ChooseDrive = new DelegateCommand.DelegateCommand(OnClick_ChooseDrive);
            Drives = new ObservableCollection<Drive>();
        }
        public DisplayedUnit unit { get; private set; }
        public ICommand ChooseDrive { get; private set; }
        public ICommand KB { get; private set; }
        public ICommand MB { get; private set; }
        public ICommand GB { get; private set; }

        public void OnClick_ChooseDrive(object obj)
        {
            VistaFolderBrowserDialog dialogBrowser = new VistaFolderBrowserDialog();
            dialogBrowser.ShowDialog();
            //Parallel 1
            var DriveData = dataProvider.ScanDirectory(dialogBrowser.SelectedPath);
            var drive = DirectoryToDrive(DriveData);
            _fsObject = DriveData;
            Name = _fsObject.Name;
            Size = _fsObject.Size;
            Path = _fsObject.Path;
            //Parallel 2
            Drives.Add(drive);
            size = DriveData.Size / 1024;
            unit = DisplayedUnit.Kilobyte;
            MessageBox.Show(size.ToString());
        }

        private Drive DirectoryToDrive(IFileSystemDirectory directory)
        {
            var modelToMap = directory;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IFileSystemDirectory, Drive>();
            });
            Mapper mapper = new Mapper(config);
            var drive = mapper.Map<IFileSystemDirectory, Drive>(modelToMap);
            return drive;
        }

        public void OnClick_KB(object obj)
        {
            if (unit == DisplayedUnit.Gigabyte)
            {
                size = size * 1024 * 1024;
            }
            else if (unit == DisplayedUnit.Megabyte)
            {
                size = size * 1024;
            }
            unit = DisplayedUnit.Kilobyte;
            MessageBox.Show(size.ToString("#.##") + " kb");
        }
        public void OnClick_MB(object obj)
        {
            if (unit == DisplayedUnit.Gigabyte)
            {
                size = size * 1024;
            }
            else if (unit == DisplayedUnit.Kilobyte)
            {
                size = size / 1024;
            }
            unit = DisplayedUnit.Megabyte;
            MessageBox.Show(size.ToString("#.##") + " mb");
        }
        public void OnClick_GB(object obj)
        {
            if (unit == DisplayedUnit.Megabyte)
            {
                size = size / 1024;
            }
            else if (unit == DisplayedUnit.Kilobyte)
            {
                size = size / 1024 / 1024;
            }
            unit = DisplayedUnit.Gigabyte;
            MessageBox.Show(size.ToString("#.##") + " gb");
        }
    }
}
