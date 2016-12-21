using MineFlags.Notification;
using System.IO;
using System.Security.Permissions;

namespace MineFlags.Storage
{
    public class Watcher : IWatcher
    {
    	private FileSystemWatcher FileWatcher { get; set; }
        private string FilePath;

        public Watcher(string filenamePath) 
        {
            FilePath = filenamePath;
        }

        public void Dispose()
        {
            FileWatcher.Changed -= handleChange;
            FileWatcher = null;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {
            if (FileWatcher != null)
                return;

            // Create a FileSystemWatcher
            FileWatcher = new FileSystemWatcher();
            FileWatcher.Path = Directory.GetCurrentDirectory();

            // Set some filters for changes
            FileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            FileWatcher.Filter = FilePath;

            // Attach callbacks
            FileWatcher.Changed += handleChange;

            // Enable the file watcher on rasing events
            FileWatcher.EnableRaisingEvents = true;
        }

        private void handleChange(object source, FileSystemEventArgs args)
        {
            if (FileWatcher != null)
                FileWatcher.EnableRaisingEvents = false;

            // Notify any listeners of the file change event
            StorageCenter.Instance.OnFileChangeEvent();

            if (FileWatcher != null)
                FileWatcher.EnableRaisingEvents = true;
        }

        public void Pause()
        {
            FileWatcher.EnableRaisingEvents = false;
        }

        public void Resume()
        {
            FileWatcher.EnableRaisingEvents = true;
        }
    }
}
