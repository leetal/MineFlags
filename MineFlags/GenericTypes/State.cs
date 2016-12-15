using System;
using MineFlags.PlayerType;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace MineFlags.GenericTypes
{
    public class State : AbstractState
    {
        public State() { }

        public State(Mine[] minefield, int rows, int columns, int mines, int remaining_mines, PlayerNum currentPlayer, Dictionary<PlayerNum, IPlayer> players)
        {
            Minefield = minefield;
            Rows = rows;
            Columns = columns;
            RemainingMines = remaining_mines;
            Mines = mines;
            CurrentPlayer = currentPlayer;
            Players = players;
        }

        public Mine[] Minefield { get; set; }
        public Dictionary<PlayerNum, IPlayer> Players;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int RemainingMines { get; set; }
        public int Mines { get; set; }
        public PlayerNum CurrentPlayer { get; set; }

        private XElement[] GetAllMines()
        {
            XElement[] MinesArr = new XElement[Minefield.Length];
            for(int i = 0; i < Minefield.Length; i++)
            {
                Mine TempMine = Minefield[i];
                MinesArr[i] = TempMine.ObjectToX();
            }
            return MinesArr;
        }

        private XElement[] GetAllPlayers()
        {
            XElement[] PlayersArr = new XElement[Players.Keys.Count];
            int i = 0;
            foreach (var player in Players)
            {
                PlayersArr[i] = player.Value.ObjectToX();
                i++;
            }
            return PlayersArr;
        }

        private Dictionary<PlayerNum, IPlayer> XToPlayers(XElement elem)
        {
            Dictionary<PlayerNum, IPlayer> playersDictionary = new Dictionary<PlayerNum, IPlayer>();
            // Toss some LINQ magic on it!
            IEnumerable<IPlayer> players = elem.Elements().Select(playerElement =>
            {
                var type = (string)playerElement.Element("type");

                // Some magic due to players being referenced as interfaces
                IPlayer TempIPlayer = null;
                switch (type)
                {
                    case "regular":
                        TempIPlayer = (IPlayer)new RegularPlayer();
                        break;
                    case "ai":
                        TempIPlayer = (IPlayer)new AIPlayer();
                        break;
                }

                if(TempIPlayer == null)
                {
                    throw new Exception("Player type unknown");
                }

                // Populate the player object with data
                TempIPlayer.XToObject(playerElement);

                return TempIPlayer;
            });

            foreach(IPlayer player in players)
            {
                // Add the newly created player to the dictionary
                playersDictionary.Add(player.GetPlayerNumber(), player);
            }

            return playersDictionary;
        }

        private Mine[] XToMineField(XElement elem, int rows, int columns)
        {
            var MinesArray = new Mine[rows * columns];
            // Toss some LINQ magic on it!
            IEnumerable<Mine> mines = elem.Elements().Select(mineElement =>
            {
                Mine TempMine = new Mine();
                // Parse the values
                TempMine.XToObject(mineElement);
                return TempMine;
            });

            int i = 0;
            foreach(Mine mine in mines)
            {
                // Add the newly created mine to the array
                MinesArray[i] = mine;
                i++;
            }

            return MinesArray;
        }

        public override XElement ObjectToX()
        {
            return new XElement("states",
                new XElement("state",
                    new XElement("mines", GetAllMines()),
                    new XElement("players", GetAllPlayers()),
                    new XElement("rows", Rows),
                    new XElement("columns", Columns),
                    new XElement("remainingmines", RemainingMines),
                    new XElement("currentplayer", (int)CurrentPlayer)
            ));
        }

        public override void XToObject(XElement elem)
        {
            // Set the values based on the state element
            Rows = (int)elem.Element("rows");
            Columns = (int)elem.Element("columns");
            RemainingMines = (int)elem.Element("remainingmines");
            CurrentPlayer = (PlayerNum)((int)elem.Element("currentplayer"));

            Players = XToPlayers(elem.Element("players"));
            Minefield = XToMineField(elem.Element("mines"), Rows, Columns);
        }
    }
}
