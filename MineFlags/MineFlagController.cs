using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    class MineFlagController
    {
        private Mine[] _minefield;  

        public MineFlagController()
        {
            Console.WriteLine("Random I am");

            _buildMinefield();
        }

        // Private methods
        private void _buildMinefield()
        {
            // Create all the mines in the minefield
            _minefield = new Mine[ROWS * COLUMNS - 1];
            for (int index = 0; index < _minefield.Length; ++index)
                _minefield[index] = new Mine();

            // Set out some mines
            Random r = new Random();
            int added_mines = 0;
            while (added_mines < MINES) {
                int index = r.Next(0, _minefield.Length - 1);
                Console.WriteLine(index);
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

            // Tell the mine above about the new mine
            {
                int row = ((y - 1) < 0) ? 0 : (y - 1);
                _minefield[(row * ROWS) + (x - 1)].increaseNeighbours();
            	_minefield[(row * ROWS) + x].increaseNeighbours();
            	_minefield[(row * ROWS) + (x + 1)].increaseNeighbours();
            }

            // Tell the one's on the sides
            _minefield[(y * ROWS) + (x - 1)].increaseNeighbours();
            _minefield[(y * ROWS) + (x + 1)].increaseNeighbours();

            // Tell the one's below
            _minefield[((y + 1) * ROWS) + (x - 1)].increaseNeighbours();
            _minefield[((y + 1) * ROWS) + x].increaseNeighbours();
            _minefield[((y + 1) * ROWS) + (x + 1)].increaseNeighbours();
        }

        // private Tuple<int, int> _getPositionForIndex(int index)
        // {
        //     return new Tuple<int, int>(0 ,0);
        // }

        private int _getRowForIndex(int index)
        {
            return (index / COLUMNS);
        }

        private int _getColumnForIndex(int index)
        {
            return index % COLUMNS;
        }

        private void _printMinefield()
        {
            for (int index = 0; index < _minefield.Length; ++index)
            {
                Console.Write("{0}", _minefield[index].toString());
            }
        }

    }
}
