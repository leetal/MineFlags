using System;
using System.IO;
using System.Security.Permissions;
using MineFlags.Logic;

namespace MineFlags.Storage
{
    public class Watcher : IWatcher
    {
    	private FileSystemWatcher FileWatcher { get; set; }
        private string Filename;
        private IController Controller;

        public Watcher(string path, IController controller) 
        {
            Controller = controller;
            Filename = path;
        }

        public void Dispose()
        {
            Controller = null;
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
            FileWatcher.Filter = Filename;

            // Attach callbacks
            FileWatcher.Changed += handleChange;

            // Enable the file watcher on rasing events
            FileWatcher.EnableRaisingEvents = true;
        }

        private void handleChange(object source, FileSystemEventArgs args)
        {
            Console.WriteLine("File: " + args.FullPath + " " + args.ChangeType);
            FileWatcher.EnableRaisingEvents = false;

            if(Controller != null)
                Controller.ResumeGameFromState();

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
