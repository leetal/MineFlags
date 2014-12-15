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
        private int _mines;

        public MineFlagController(int rows, int columns, int mines)
        {
            _rows = rows;
            _columns = columns;
            _mines = mines;

            _buildMinefield();
        }

        // Private methods
        private void _buildMinefield()
        {
            // Create all the mines in the minefield
            _minefield = new Mine[_rows * _columns - 1];
            for (int index = 0; index < _minefield.Length; ++index)
                _minefield[index] = new Mine();

            // Set out some mines
            Random r = new Random();
            int added_mines = 0;
            while (added_mines < _mines) {
                int index = r.Next(0, _minefield.Length - 1);
                Mine mine = _minefield[index];

                /* If the mine already is a mine, try again */
                if (mine.isMine())
                    continue;

                mine.setAsMine(true);
                _updateAroundMine(index);
                ++added_mines;
            }

            /* Print the array with mines */
            _printMinefield();
        } 

        private void _updateAroundMine(int index)
        {
            int y = _getRowForIndex(index);
            int x = _getColumnForIndex(index);

            // Tell the mines above about the new mine
            {
                int row = ((y - 1) < 0) ? 0 : (y - 1) * _rows;
                _minefield[row + ((x - 1) < 0 ? 0 : (x - 1)].increaseNeighbours();
                _minefield[row + x].increaseNeighbours();
                _minefield[row + ((x + 1) >= _rows].increaseNeighbours();
            }

            // Tell the one's on the sides
            {
                int row = y * _rows;
                _minefield[row + (((x - 1) < 0) ? 0 : (x - 1))].increaseNeighbours();
                _minefield[row + (((x + 1) >= _columns) ? 0 : (x + 1))].increaseNeighbours();
            }

            // Tell the one's below
            {
                int row = ((y + 1) >= _rows) ? _rows : (y + 1);
                _minefield[row + (x - 1)].increaseNeighbours();
                _minefield[row + x].increaseNeighbours();
                _minefield[row + (x + 1)].increaseNeighbours();
            }
        }

        // private Tuple<int, int> _getPositionForIndex(int index)
        // {
        //     return new Tuple<int, int>(0 ,0);
        // }

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

                Console.Write("{0}", _minefield[index].toString());
            }
        }

    }
}
