using System;
using System.IO;
using System.Xml.Serialization;
using MineFlags.GenericTypes;
using System.Xml.Linq;

namespace MineFlags.Storage
{
    public class StateHandler
    {   

        public static void DeleteStorageIfExists(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }

        public static void ExportToStorage(IStorageEntity state, string filename)
        {
            // Save to file
            XDocument stateTree = new XDocument(
                new XComment("This file holds the current state of the game"),
                state.ObjectToX()
            );
            stateTree.Save(filename);
        }

        public static State ImportFromStorage(string filename)
        {
            XDocument xdoc = XDocument.Load(filename);

            // NOTE: The format specifies that we shall always have just one state under states.
            XElement stateElem = xdoc.Root.Element("state");

            // The below construct is so that we can switch implementations later on without too much hussle
            return AbstractState.CreateInstance<State>(stateElem);
        }
    }
}
