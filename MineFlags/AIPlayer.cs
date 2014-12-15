using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    class AIPlayer
    {
        private int[] _minefield;
        AIPlayer(int rows, int columns) {
            /* Reset the minefield */
            _minefield = Enumerable.Repeat<int>(0, MineField.ROWS * MineField.COLUMNS).ToArray(); // new int[rows * columns];

            MineFlagController.onMineOpened += onMineOpened;
        }

        private void _resetMinefield() {

        }

        public void onMineOpened(Mine m) {
            Console.WriteLine("[AIPlayer] onMineOpened");
        }
    }
}
