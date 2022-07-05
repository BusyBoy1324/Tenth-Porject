using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TenthProject.Models;
using Directory = TenthProject.Models.Directory;
using File = TenthProject.Models.File;

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
                _collectionMutex.WaitOne();
                Models.File[] result = _scannedObjects.ToArray();
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
                        fsDirectory.Files++;
                        try
                        {
                            FileInfo fileInfo = new FileInfo(t);
                            fsDirectory.NestedObjects.Add(new Models.File(fileInfo));
                            SafeAddToScannedObjects(new Models.File(fileInfo));
                            var size = fileInfo.Length / 1024;
                            fsDirectory.Size += (ulong)size;
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }
                }
            }); 

            for (int i = 0; i < directoriesPromises.Count; i++)
            {
                try
                {
                    Models.Directory dir = await directoriesPromises[i];
                    fsDirectory.NestedObjects.Add(dir);
                    fsDirectory.Size += dir.Size;
                    fsDirectory.Files += dir.Files;
                }
                catch(Exception ex)
                {
                    int s = 0;
                }
            }
            return fsDirectory;
        }

        public static Directory StartSync(string path)
        {
            _stopScanning = false;
            var result = ScanDirectory(path).Result;
            return result;
        }

        public static void ResumeScanning()
        {
            _stopScanning = true;
        }
    }
}
