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
        public String path;

        public Watcher(String path) { }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {
            // Create a FileSystemWatcher
            watcher = new FileSystemWatcher();
            watcher.Path = Directory.GetCurrentDirectory();

            // Set some filters for changes
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*";

            // Attach callbacks
            watcher.Changed += handleChange;

            // Somebody's watching yoooou...
            watcher.EnableRaisingEvents = true;
        }

        private void handleChange(object source, FileSystemEventArgs args)
        {
            Console.WriteLine("File: " + args.FullPath + " " + args.ChangeType);
        }
    }
}
