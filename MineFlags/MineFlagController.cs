using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    public enum Player
    {
        ONE = 0,
        TWO = 1
    };

    class MineFlagController
    {
        private Mine[] _minefield;
        private int _rows;
        private int _columns;
        private int _remaining_mines;

        private int[] _scores = new int[2] { 0, 0 };
        private Player _current_player_turn = Player.ONE;

        // Delegates
        public delegate void MinefieldHandler();
        public delegate void TurnHandler(Player player);
        public delegate void MineHandler(Mine m);

        // Events
        public static event MineHandler onMineOpened;
        public static event MinefieldHandler onResetMinefield;
        public static event TurnHandler announceTurn;

        public MineFlagController(int rows, int columns, int mines)
        {
            _rows = rows;
            _columns = columns;
            _remaining_mines = mines;

            /* Add our _printMinefield as an EventListener */
            onResetMinefield += _printMinefield;

            _buildMinefield();

            // Announce the turn directly
            announceTurn(_current_player_turn);
        }

        public void openMine(int index) {
            Mine mine = _minefield[index];
            if (mine.isOpened())
                return;

            /* Open the mine and always change turns */
            mine.open();
            _changeTurns();

            if (!mine.isMine() && mine.getNeighbours() == 0) {
                /* Reveal all neighbouring mines if the mine has an value of 0 */
                if (!mine.isOpened() && !mine.isMine()) {
                    mine.open();
                    _openNeighbouringMines(mine.index);
                }
                _openNeighbouringMines(index);
            } else if (mine.isMine()) {
                /* Up the score of the one who took it */
                _scores[(int)_current_player_turn] += 1;

                /* If we found a mine it's our turn again */
                _changeTurns();
            }

            /* Notify everyone about the opened mine */
            if (onMineOpened != null) {
                onMineOpened(mine);
            }

            // Notify everyone about the turn
            announceTurn(_current_player_turn);
        }

        private void _buildMinefield()
        {
            // Create all the mines in the minefield
            _minefield = new Mine[_rows * _columns];
            for (int index = 0; index < _minefield.Length; ++index)
                _minefield[index] = new Mine((index / _columns), (index % _columns));

            // Set out some mines
            Random r = new Random();
            int added_mines = 0;
            while (added_mines < _remaining_mines) {
                int index = r.Next(0, _minefield.Length - 1);
                Mine mine = _minefield[index];

                /* If the mine already is a mine, try again */
                if (mine.isMine())
                    continue;

                mine.setAsMine(true);

                /* Tell the neighbours about the newly added mine */
                List<Mine> mines = _getNeighbouringMines(index, true);
                mines.ForEach(delegate(Mine m) {
                    m.increaseNeighbours();
                });

                ++added_mines;
            }

            if (onResetMinefield != null)
                onResetMinefield();
        } 

        //private void _updateAroundMine(int index)
        private List<Mine> _getNeighbouringMines(int index, bool corners)
        {
            List<Mine> mines = new List<Mine>();
            int y = _getRowForIndex(index);
            int x = _getColumnForIndex(index);

            // Get the mines above about the new mine
            {
                int row = y - 1;
                if (row >= 0) {
                    if (corners && (x - 1) >= 0) mines.Add(_minefield[row * _rows + (x - 1)]);
                    if (corners && (x + 1) < _columns) mines.Add(_minefield[row * _rows + (x + 1)]);
                    mines.Add(_minefield[row * _rows + x]);
                }
            }

            // Get the one's on the sides
            {
                int row = y;
                if ((x - 1) >= 0) mines.Add(_minefield[row * _rows + (x - 1)]);
                if ((x + 1) < _columns) mines.Add(_minefield[row * _rows + (x + 1)]);
            }

            // Get the one's below
            {
                int row = (y + 1);
                if (row < _rows)
                {
                    if (corners && (x - 1) >= 0) mines.Add(_minefield[row * _rows + (x - 1)]);
                    if (corners && (x + 1) < _columns) mines.Add(_minefield[row * _rows + (x + 1)]);
                    mines.Add(_minefield[row * _rows + x]);
                }
            }

            return mines;
        }

        private int _getRowForIndex(int index)
        {
            return (index / _columns);
        }

        private int _getColumnForIndex(int index)
        {
            return index % _columns;
        }

        private void _printMinefield()
        {
            for (int index = 0; index < _minefield.Length; ++index)
            {
                if ((index % _columns) == 0)
                    Console.Write("\n");

                Console.Write(_minefield[index].toString());
            }
        }

        private void _openNeighbouringMines(int index)
        {
            Mine mine = _minefield[index];

            if (!mine.isMine() && mine.getNeighbours() == 0) {
                List<Mine> mines = _getNeighbouringMines(index, true);
                foreach (Mine m in mines) {
                    if (!m.isOpened() && !m.isMine()) {
                        m.open();
                        onMineOpened(m);
                        _openNeighbouringMines(m.index);
                    }
                }
            }
        }

        private void _changeTurns()
        {
            if (_current_player_turn == Player.ONE)
                _current_player_turn = Player.TWO;
            else
                _current_player_turn = Player.ONE;
        }

    }
}
