using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AutoMapper;
using Ookii.Dialogs.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
        public ObservableCollection<IFileSystemObject> Files { get; set; }
        private static ConcurrentQueue<Models.File> _newFiles { get; set; }
        public ObservableCollection<Drive> Drives { get; set; }
        private Dispatcher _dispatcher;
        private string _path;
        public static long size;
        private static Drive? _drive;
        public MainWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Files = new ObservableCollection<IFileSystemObject>();
            _newFiles = new ConcurrentQueue<File>();
            worker_Init();
            KB = new DelegateCommand.DelegateCommand(OnClick_KB);
            MB = new DelegateCommand.DelegateCommand(OnClick_MB);
            GB = new DelegateCommand.DelegateCommand(OnClick_GB);
            ChooseDrive = new DelegateCommand.DelegateCommand(OnClick_ChooseDrive);
            Drives = new ObservableCollection<Drive>();
            Analyzer.Analyzer.ScannedObjectsAdded += ObjectsPropertyChanged;
        }
        public DisplayedUnit unit { get; private set; }
        public ICommand ChooseDrive { get; private set; }
        public ICommand KB { get; private set; }
        public ICommand MB { get; private set; }
        public ICommand GB { get; private set; }

        private static void ObjectsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var f = Analyzer.Analyzer.ScannedObjects;
            for (int i = 0; i < f.Length; i++)
            {
                _newFiles.Enqueue(f[i]);
            }
        }
        public void OnClick_ChooseDrive(object obj)
        {
            VistaFolderBrowserDialog dialogBrowser = new VistaFolderBrowserDialog();
            dialogBrowser.ShowDialog();
            _path = dialogBrowser.SelectedPath;
            Thread thread = new Thread(() =>
                Analyzer.Analyzer.StartSync(dialogBrowser.SelectedPath));
            thread.Start();

            unit = DisplayedUnit.Kilobyte;
        }
        private void worker_Init()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Files = new ObservableCollection<IFileSystemObject>();
            while (true)
            {
                if (_newFiles.Count > 0)
                {
                    _dispatcher.Invoke(new Action(() =>
                    {
                        File file = null;
                        while (!_newFiles.TryDequeue(out file))
                        {
                            Thread.Sleep(5);
                        }
                        ParseObject(file);
                    }));
                    Thread.Sleep(5);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {

        }

        private void ParseObject(File file)
        {
            if (file == null)
            {
                return;
            }
            string[] folders = null;
            {
                string path = file.Path.Remove(0, _path.Length);
                if (path.IndexOf('\\') == 0)
                {
                    path = path.Remove(0, 1);
                }

                int indexOfFile = path.LastIndexOf('\\');
                if (indexOfFile == -1)
                {
                    Files.Add(file);
                    return;
                }

                folders = path.Remove(indexOfFile).Split('\\');
            }
            ObservableCollection<IFileSystemObject> nestedObjects = Files;
            for (int i = 0; i < folders.Length; i++)
            {
                IFileSystemObject fsObject = null;
                for (int j = 0; j < nestedObjects.Count; j++)
                {
                    if (folders[i] == nestedObjects[j].Name)
                    {
                        fsObject = nestedObjects[j];
                        break;
                    }
                }
                if (fsObject == null)
                {
                    fsObject = new Directory(folders[i]);
                    nestedObjects.Add(fsObject);
                }

                if (fsObject is File)
                {
                    nestedObjects.Remove(fsObject);
                    fsObject = new Directory(folders[i]);
                }

                fsObject.Size += file.Size;
                (fsObject as Directory).Files += 1;
                nestedObjects = (fsObject as Directory).NestedObjects;
            }

            nestedObjects.Add(file);
        }
        //public void GetDriveData(VistaFolderBrowserDialog dialogBrowser)
        //{
        //    var driveData = Analyzer.Analyzer.StartSync(dialogBrowser.SelectedPath);
        //    _drive = DirectoryToDrive(driveData);
        //}

        //public void FillObservableCollection()
        //{
        //    ObservableCollection<Drive> _drives = new ObservableCollection<Drive>();
        //    Drive temp = null;
        //    while (true)
        //    {
        //        Thread.Sleep(200);
        //        if (_drive != temp)
        //        {
        //            temp = _drive;
        //            _drives.Add(_drive);
        //        }
        //    }
        //}

        //private Drive DirectoryToDrive(IFileSystemObject directory)
        //{
        //    var modelToMap = directory;
        //    var config = new MapperConfiguration(cfg =>
        //    {
        //        cfg.CreateMap<IFileSystemObject, Drive>();
        //    });
        //    Mapper mapper = new Mapper(config);
        //    var drive = mapper.Map<IFileSystemObject, Drive>(modelToMap);
        //    return drive;
        //}

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
