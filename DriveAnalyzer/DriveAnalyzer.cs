using Models;

namespace DriveAnalyzer
{
    public class DriveAnalyzer
    {
        public static async Task<IFileSystemDirectory> ScanDirectory(string path)
        {
            Models.Directory fsDirectory = new Models.Directory(path.Substring(path.LastIndexOf('\\') + 1), path);
            string[] fsObjects;
            try
            {
                fsObjects = System.IO.Directory.GetFileSystemEntries(path);
            }
            catch (Exception ex)
            {
                return fsDirectory;
            }

            List<Task<IFileSystemDirectory>> directoriesPromises = new List<Task<IFileSystemDirectory>>();
            for (var i = 0; i < fsObjects.Length; i++)
            {
                if (System.IO.Directory.Exists(fsObjects[i]))
                {
                    directoriesPromises.Add(ScanDirectory(fsObjects[i]));
                }
                else if (System.IO.File.Exists(fsObjects[i]))
                {
                    FileInfo fileInfo;
                    try
                    {
                        fileInfo = new FileInfo(fsObjects[i]);
                        fsDirectory.Childrens.Add(new Models.File(fileInfo));
                        fsDirectory.Size += fileInfo.Length;
                    }
                    catch (Exception ex)
                    {
                        fsDirectory.Childrens.Add(new Models.File(fsObjects[i].Substring(fsObjects[i].LastIndexOf('\\') + 1), path: fsObjects[i]));
                    }
                }
            }
            for (int i = 0; i < directoriesPromises.Count; i++)
            {
                IFileSystemObject dir = await directoriesPromises[i];
                fsDirectory.Childrens.Add(dir);
                fsDirectory.Size += dir.Size;
            }

            return fsDirectory;
        }
    }
}