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

            // Since the format specified only contains one state, this should be fine in all (current) cases
            /*State state =
                (
                    from s in doc.Root.Elements("state")
                    select new State
                    {
                        Rows = (int)s.Element("rows"),
                        Columns = (int)s.Element("columns"),
                        RemainingMines = (int)s.Element("remainingmines"),
                        CurrentPlayer = (PlayerNum)((int)s.Element("currentplayer")),
                        Players = XToPlayers(s.Document),
                        Minefield = XToMineField(s.Document, (int)s.Element("rows"), (int)s.Element("columns")),
                    }
                ).FirstOrDefault();

            // Set the variables in THIS instance
            Rows = state.Rows;
            Columns = state.Columns;
            RemainingMines = state.RemainingMines;
            CurrentPlayer = state.CurrentPlayer;
            */

            // The below construct is so that we can switch implementations later on without too much hussle
            return AbstractState.CreateInstance<State>(stateElem);
        }
    }
}
