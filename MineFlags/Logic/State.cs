using System;
using MineFlags.PlayerType;
using MineFlags.GenericTypes;
using System.Collections.Generic;

namespace MineFlags.Logic
{
    [Serializable]
    public class State
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
    }
}
