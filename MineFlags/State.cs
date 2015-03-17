using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    [Serializable]
    public class State
    {
        public State() { }

        public State(List<Mine> minefield, int rows, int columns, int mines, int remaining_mines, int[] scores, Player current_player_turn, AIPlayer ai)
        {
            this.minefield = minefield;
            this.rows = rows;
            this.columns = columns;
            this.remaining_mines = remaining_mines;
            this.mines = mines;
            this.scores = scores;
            this.current_player_turn = current_player_turn;
            this.ai = ai;
        }

        public List<Mine> minefield { get; set; }
        public AIPlayer ai { get; set; }
        public int rows { get; set; }
        public int columns { get; set; }
        public int remaining_mines { get; set; }
        public int mines { get; set; }
        public int[] scores { get; set; }
        public Player current_player_turn { get; set; }
    }
}
