using System;
using System.IO;
using System.Xml.Serialization;
using MineFlags.GenericTypes;
using System.Xml.Linq;
using MineFlags.Exceptions;

namespace MineFlags.Storage
{
    public class StateHandler : IStateHandler
    {
        private string FilePath;
        private IWatcher FileWatcher;

        public StateHandler(string filePath, IWatcher fileWatcher)
        {
            FilePath = filePath;
            FileWatcher = fileWatcher;

            // Add a guard so that we never get into a state where we cannot do everything
            if (FilePath == "" || FileWatcher == null)
                throw new InvalidDIException("Storage handlers have not been initialized");

            // Start the file watcher
            FileWatcher.Run();
        }

        public void Dispose()
        {
            FileWatcher.Dispose();
        }

        public bool StorageExists()
        {
            return File.Exists(FilePath);
        }

        public void DeleteStorageIfExists()
        {
            if (StorageExists())
                File.Delete(FilePath);
        }

        public void ExportToStorage(IStorageEntity state)
        {
            try
            {
                // We need to pause the file watcher since this change otherwise will trigger an event
                FileWatcher.Pause();

                // Convert the objects to XElements
                XDocument stateTree = new XDocument(
                    new XComment("This file holds the current state of the game"),
                    state.ObjectToX()
                );
                // Save the file!
                stateTree.Save(FilePath);

                // Resume the file watcher
                FileWatcher.Resume();
            }
            catch (Exception e)
            {
                // Cach-all at lowest tier
                // Throw new exception upwards
                throw new StateException("Storage failed to save the state", e);
            }
        }

        public State ImportFromStorage()
        {
            try
            {
                XDocument xdoc = XDocument.Load(FilePath);

                // NOTE: The format specifies that we shall always have just one state under states.
                XElement stateElem = xdoc.Root.Element("state");

                // The below construct is so that we can switch implementations later on without too much hussle

                return AbstractState.CreateInstance<State>(stateElem);
            }
            catch (Exception e)
            {
                // Cach-all at lowest tier
                // Throw new exception upwards
                throw new StateException("Failed to retrieve the state", e);
            }
        }
    }
}
