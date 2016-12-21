using MineFlags.GenericTypes;
using MineFlags.PlayerType;
using System;

namespace MineFlags.Notification
{
    public sealed class StorageCenter
    {
        // Provide a multithreaded singleton for convenience when working with events
        private static volatile StorageCenter instance;
        private static object syncRoot = new Object(); // Used as a mutex in this context

        private StorageCenter() { }

        public static StorageCenter Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new StorageCenter();
                    }
                }

                return instance;
            }
        }

        // Delegates + Events manage the data bindings
        // Delegates
        public delegate void FileChangeType();
        
        // Events, static for convenience
        public event FileChangeType FileChangeEvent;

        // These are here for convenience when invoking an event
        public void OnFileChangeEvent()
        {
            FileChangeEvent?.Invoke();
        }
    }
}
