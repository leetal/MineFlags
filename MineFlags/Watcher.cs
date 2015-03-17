using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.Permissions;

namespace MineFlags
{
    public class Watcher
    {
    	public FileSystemWatcher watcher { get; set; }
        private String _filename;
        private MineFlagController _controller;

        public Watcher(String path, MineFlagController controller) 
        {
            _controller = controller;
            _filename = path;
        }

        public void Dispose()
        {
            _controller = null;
            watcher.Changed -= handleChange;
            watcher = null;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {
            // Create a FileSystemWatcher
            watcher = new FileSystemWatcher();
            watcher.Path = Directory.GetCurrentDirectory();

            // Set some filters for changes
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = _filename;

            // Attach callbacks
            watcher.Changed += handleChange;

            // Somebody's watching yoooou...
            watcher.EnableRaisingEvents = true;
        }

        private void handleChange(object source, FileSystemEventArgs args)
        {
            Console.WriteLine("File: " + args.FullPath + " " + args.ChangeType);
            watcher.EnableRaisingEvents = false;
            if(_controller != null)
                _controller.ContinueGame();
            watcher.EnableRaisingEvents = true;
        }
    }
}
