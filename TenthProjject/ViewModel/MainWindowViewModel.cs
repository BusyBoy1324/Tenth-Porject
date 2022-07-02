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
        private static Queue<Models.File> NewFiles { get; set; }
        public ObservableCollection<Drive> Drives { get; set; }
        private Dispatcher _dispatcher;
        private string _path;
        public static long size;
        private static Drive? _drive;
        private Thread _scanThread;
        private BackgroundWorker _worker;
        public MainWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Files = new ObservableCollection<IFileSystemObject>();
            NewFiles = new Queue<File>();
            worker_Init();
            KB = new DelegateCommand.DelegateCommand(OnClick_KB);
            MB = new DelegateCommand.DelegateCommand(OnClick_MB);
            GB = new DelegateCommand.DelegateCommand(OnClick_GB);
            ChooseDrive = new DelegateCommand.DelegateCommand(OnClick_ChooseDrive);
            Drives = new ObservableCollection<Drive>();
            Analyzer.Analyzer.ScannedObjectsAdded += ObjectsPropertyChanged;
            _scanThread = null;
        }
        public DisplayedUnit unit { get; private set; }
        public ICommand ChooseDrive { get; private set; }
        public ICommand KB { get; private set; }
        public ICommand MB { get; private set; }
        public ICommand GB { get; private set; }

        private void ObjectsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var f = Analyzer.Analyzer.ScannedObjects;
            for (int i = 0; i < f.Length; i++)
            {
                _dispatcher.Invoke(() =>
                {
                    ParseObject(f[i]);
                });
            }
        }
        public void OnClick_ChooseDrive(object obj)
        {
            VistaFolderBrowserDialog dialogBrowser = new VistaFolderBrowserDialog();
            dialogBrowser.ShowDialog();
            _path = dialogBrowser.SelectedPath;
            _scanThread = new Thread(() =>
            {
                var final = Analyzer.Analyzer.StartSync(dialogBrowser.SelectedPath);
                _worker.Dispose();
                _dispatcher.Invoke(() =>
                {
                    Files.Clear();
                    for (int i = 0; i < final.NestedObjects.Count; i++)
                    {
                        Files.Add(final.NestedObjects[i]);
                    }
                });
            });
            _scanThread.Start();
            unit = DisplayedUnit.Kilobyte;
        }
        private void worker_Init()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += worker_DoWork;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerAsync();
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

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
                (fsObject as Directory).Files++;
                nestedObjects = (fsObject as Directory).NestedObjects;
            }
            nestedObjects.Add(file);
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
