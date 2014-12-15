using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    class MineFlagController
    {
        private Mine[] _minefield;
        private int _rows;
        private int _columns;
        private int _remaining_mines;

        private int[] _scores = new int[2] { 0, 0 };

        public MineFlagController(int rows, int columns, int mines)
        {
            _rows = rows;
            _columns = columns;
            _remaining_mines = mines;

            /* Add our _printMinefield as an EventListener */
            onResetMinefield += _printMinefield;

            _buildMinefield();
        }

        public delegate void MineHandler(Mine m);
        public static event MineHandler onMineOpened;
        public void openMine(int index) {
            Mine mine = _minefield[index];
            if (mine.isOpened())
                return;

            mine.open();

            if (!mine.isMine() && mine.getNeighbours() == 0) {
                Console.WriteLine("mine.isEmpty() == true");

                /* Reveal all neighbouring mines if the mine has an value of 0 */
                List<Mine> mines = _getNeighbouringMines(index, true);
                foreach (Mine m in mines) {
                    Console.WriteLine(m.index);
                    if (!m.isOpened() && !m.isMine()) {
                        openMine(m.index);
                    }
                }
            } else if (mine.isMine()) {
                /* Up the score of the one who took it */
            }

            /* Notify everyone about the opened mine */
            if (onMineOpened != null) {
                Console.WriteLine("SENDING CALLBACKS ON OPENMINE");
                onMineOpened(mine);
                }
        }

        // Private methods
        public delegate void MinefieldHandler();
        public static event MinefieldHandler onResetMinefield;
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

    }
}
