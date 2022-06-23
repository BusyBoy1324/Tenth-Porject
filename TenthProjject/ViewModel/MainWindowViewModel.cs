using AutoMapper;
using Microsoft.Win32;
using Models;
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
        public ObservableCollection<TreeViewModel> _children;
        public ObservableCollection<Drive> Drives;
        public ObservableCollection<TreeViewModel> Children
        {
            get { return _children; }
        }
        public DirectoryViewModel directoryViewModel;
        public string Name => directoryViewModel.Name;
        public long Size => directoryViewModel.Size;
        public static long size;
        private AlalyzeProvider dataProvider = new AlalyzeProvider();
        public MainWindowViewModel()
        {
            KB = new DelegateCommand.DelegateCommand(OnClick_KB);
            MB = new DelegateCommand.DelegateCommand(OnClick_MB);
            GB = new DelegateCommand.DelegateCommand(OnClick_GB);
            ChooseDrive = new DelegateCommand.DelegateCommand(OnClick_ChooseDrive);
            _children = new ObservableCollection<TreeViewModel>();
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
            var DriveData = dataProvider.ScanDirectory(dialogBrowser.SelectedPath);
            var modelTomapp = DriveData;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IFileSystemObject, DirectoryViewModel>();
            });
            Mapper mapper = new Mapper(config);
            directoryViewModel = mapper.Map<IFileSystemObject, DirectoryViewModel>(modelTomapp);
            ObservableCollection<TreeViewModel> childrens = GetObservableCollection(DriveData.Childrens);
            foreach (TreeViewModel model in childrens)
            {
                directoryViewModel.Children.Add(model);
                directoryViewModel.Children[directoryViewModel.Children.Count-1].Parent = directoryViewModel;
            }
            _children.Add(directoryViewModel);
            _fsObject = DriveData;
            size = DriveData.Size / 1024;
            unit = DisplayedUnit.Kilobyte;
            MessageBox.Show(size.ToString());
        }

        private TreeViewModel ConvertFromIFileSystemObject(IFileSystemObject fsObject)
        {
            var treeModel= new TreeViewModel();
            treeModel.Name = fsObject.Name;
            treeModel.Size = fsObject.Size;
            treeModel.Path= fsObject.Path;
            if(fsObject is IFileSystemDirectory)
            {
                IFileSystemDirectory fsDirectory = fsObject as IFileSystemDirectory;
                for (int i = 0; i < fsDirectory.Childrens.Count; i++)
                {
                    treeModel.Children.Add(ConvertFromIFileSystemObject(fsDirectory.Childrens[i]));
                    treeModel.Children[i].Parent = treeModel;
                }
            }
            return treeModel;
        }

        private ObservableCollection<TreeViewModel> GetObservableCollection(ObservableCollection<IFileSystemObject> fsObject)
        {
            ObservableCollection<TreeViewModel> result = new ObservableCollection<TreeViewModel>(); 
            foreach (IFileSystemObject fsObjectItem in fsObject)
            {
                result.Add(ConvertFromIFileSystemObject(fsObjectItem));
            }
            return result;
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
