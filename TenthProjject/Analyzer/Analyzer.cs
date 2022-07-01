using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TenthProject.Models;

namespace TenthProject.Analyzer
{
    public class Analyzer
    {
        private static List<Models.File> _scannedObjects = new List<Models.File>();
        private static bool _stopScanning = false;


        private static void SafeAddToScannedObjects(Models.File file)
        {
            _collectionMutex.WaitOne();
            _scannedObjects.Add(file);
            _collectionMutex.ReleaseMutex();
            OnPropertyChanged(nameof(ScannedObjects));
        }

        public static Models.File[] ScannedObjects
        {
            get
            {
                Models.File[] result = _scannedObjects.ToArray();
                _collectionMutex.WaitOne();
                _scannedObjects.Clear();
                _collectionMutex.ReleaseMutex();
                return result;
            }
        }

        public static event PropertyChangedEventHandler ScannedObjectsAdded;

        private static void OnPropertyChanged(string propertyName)
        {
            ScannedObjectsAdded?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        private static Mutex _userIdMutex = new Mutex();
        private static Mutex _collectionMutex = new Mutex();

        private static async Task<Models.Directory> ScanDirectory(string path)
        {
            Models.Directory fsDirectory = new Models.Directory(path.Substring(path.LastIndexOf('\\') + 1));
            string[] fsObjects;
            try
            {
                fsObjects = System.IO.Directory.GetFileSystemEntries(path);
            }
            catch (Exception)
            {
                return fsDirectory;
            }

            List<Task<Models.Directory>> directoriesPromises = new List<Task<Models.Directory>>();
            Parallel.ForEach(fsObjects, t =>
            {
                if (System.IO.Directory.Exists(t))
                {
                    if (!_stopScanning)
                    {
                        directoriesPromises.Add(ScanDirectory(t));
                    }
                }
                else if (System.IO.File.Exists(t))
                {

                    if (!_stopScanning)
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(t);
                            fsDirectory.NestedObjects.Add(new Models.File(fileInfo));
                            SafeAddToScannedObjects(new Models.File(fileInfo));
                            fsDirectory.Size += (ulong)fileInfo.Length;
                        }
                        catch (Exception)
                        {
                            
                        }
                    }
                }
            });

            for (int i = 0; i < directoriesPromises.Count; i++)
            {
                Models.IFileSystemObject dir = await directoriesPromises[i];
                fsDirectory.NestedObjects.Add(dir);
                fsDirectory.Size += dir.Size;
            }
            return fsDirectory;
        }

        public static void StartSync(string path)
        {
            _stopScanning = false;
            var f = ScanDirectory(path).Result;
        }

        public static void ResumeScanning()
        {
            _stopScanning = true;
        }
    }
}
