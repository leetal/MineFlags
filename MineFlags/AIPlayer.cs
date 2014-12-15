using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineFlags
{
    class AIPlayer
    {
        AIPlayer() {
            MineFlagController.onMineOpened += onMineOpened;
        }

        public void onMineOpened(Mine m) {
            Console.WriteLine("[AIPlayer] onMineOpened");
        }
    }
}
