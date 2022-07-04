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
        private Queue<File> NewFiles { get; set; }
        private Dispatcher _dispatcher;
        private string _path;
        private Thread _scanThread;
        private BackgroundWorker _worker;
        private Directory _finalDirectory;
        public MainWindowViewModel()
        {
            NewFiles = new Queue<File>();
            worker_Init();  
            Analyzer.Analyzer.ScannedObjectsAdded += ObjectsPropertyChanged;
            Files = new ObservableCollection<IFileSystemObject>();
            _dispatcher = Dispatcher.CurrentDispatcher;
            KB = new DelegateCommand.DelegateCommand(OnClick_KB);
            MB = new DelegateCommand.DelegateCommand(OnClick_MB);
            GB = new DelegateCommand.DelegateCommand(OnClick_GB);
            ChooseDrive = new DelegateCommand.DelegateCommand(OnClick_ChooseDrive);
            _scanThread = null;
            unit = DisplayedUnit.Kilobyte;
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
                _finalDirectory = Analyzer.Analyzer.StartSync(dialogBrowser.SelectedPath);
                _worker.Dispose();
                _dispatcher.Invoke(() =>
                {
                    Files.Clear();
                    for (int i = 0; i < _finalDirectory.NestedObjects.Count; i++)
                    {
                        Files.Add(_finalDirectory.NestedObjects[i]);
                    }
                });
            });
            _scanThread.Start();
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
                    if (nestedObjects[j].Name == folders[i])
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

                (fsObject as Directory).Files++;
                fsObject.Size += file.Size;
                nestedObjects = (fsObject as Directory).NestedObjects;
            }
            nestedObjects.Add(file);
        }

        public void OnClick_KB(object obj)
        {
            if (unit == DisplayedUnit.Gigabyte && _finalDirectory != null)
            {
                Files.Clear();
                for (int i = 0; i < _finalDirectory.NestedObjects.Count; i++)
                {
                    _finalDirectory.NestedObjects[i].Size = _finalDirectory.NestedObjects[i].Size * 1024 * 1024;
                    Files.Add(_finalDirectory.NestedObjects[i]);
                }
            }
            else if (unit == DisplayedUnit.Megabyte && _finalDirectory != null)
            {
                Files.Clear();
                for (int i = 0; i < _finalDirectory.NestedObjects.Count; i++)
                {
                    _finalDirectory.NestedObjects[i].Size = _finalDirectory.NestedObjects[i].Size * 1024;
                    Files.Add(_finalDirectory.NestedObjects[i]);
                }
            }
            unit = DisplayedUnit.Kilobyte;
        }
        public void OnClick_MB(object obj)
        {
            if (unit == DisplayedUnit.Gigabyte && _finalDirectory != null)
            {
                Files.Clear();
                for (int i = 0; i < _finalDirectory.NestedObjects.Count; i++)
                {
                    _finalDirectory.NestedObjects[i].Size = _finalDirectory.NestedObjects[i].Size * 1024;
                    Files.Add(_finalDirectory.NestedObjects[i]);
                }
            }
            else if (unit == DisplayedUnit.Kilobyte && _finalDirectory != null)
            {
                Files.Clear();
                for (int i = 0; i < _finalDirectory.NestedObjects.Count; i++)
                {
                    _finalDirectory.NestedObjects[i].Size = _finalDirectory.NestedObjects[i].Size / 1024;
                    Files.Add(_finalDirectory.NestedObjects[i]);
                }
            }
            unit = DisplayedUnit.Megabyte;
        }
        public void OnClick_GB(object obj)
        {
            if (unit == DisplayedUnit.Megabyte && _finalDirectory != null)
            {
                Files.Clear();
                for (int i = 0; i < _finalDirectory.NestedObjects.Count; i++)
                {
                    _finalDirectory.Size = _finalDirectory.Size / 1024;
                    Files.Add(_finalDirectory.NestedObjects[i]);
                }
            }
            else if (unit == DisplayedUnit.Kilobyte && _finalDirectory != null)
            {
                Files.Clear();
                for (int i = 0; i < _finalDirectory.NestedObjects.Count; i++)
                {
                    _finalDirectory.NestedObjects[i].Size = _finalDirectory.NestedObjects[i].Size / 1024 / 1024;
                    Files.Add(_finalDirectory.NestedObjects[i]);
                }
            }
            unit = DisplayedUnit.Gigabyte;
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {

        }
    }
}
