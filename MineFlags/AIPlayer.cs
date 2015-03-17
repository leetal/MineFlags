using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    [Serializable]
    public class AIPlayer
    {
        private MineFlagController _controller;
        public bool[] _opened_minefield { get; set; }

        public AIPlayer() { }

        public AIPlayer(MineFlagController controller, int rows, int columns) {
            /* Keep the reference to the controller */
            _controller = controller;

            /* Reset the minefield */
            _opened_minefield = Enumerable.Repeat<bool>(false, MineField.ROWS * MineField.COLUMNS).ToArray();

            MineFlagController.onMineOpened += onMineOpened;
            MineFlagController.announceTurn += _handleTurn;
        }

        public void Dispose()
        {
            MineFlagController.onMineOpened -= onMineOpened;
            MineFlagController.announceTurn -= _handleTurn;
            _controller = null;
        }

        public void onMineOpened(Mine m) {
            Console.WriteLine("[AIPlayer] onMineOpened");
            _opened_minefield[m.index] = true;
        }

        private void _handleTurn(Player p) {
            if (p == Player.ONE)
                return;

            Random r = new Random();
            for (;;) {
                int index = r.Next(0, _opened_minefield.Length - 1);

                /* We can't open that one, try again */
                if (_opened_minefield[index])
                    continue;

                _controller.openMine(index);
                break;
            }
        }
    }
}
