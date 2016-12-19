using System;
using System.Linq;
using System.Xml.Linq;
using MineFlags.GenericTypes;
using MineFlags.Logic;

namespace MineFlags.PlayerType
{
    public class AIPlayer : AbstractPlayer
    {
        private bool[] OpenedMinefield { get; set; }

        public AIPlayer() : base() { }

        public AIPlayer(int score, PlayerNum num) : base(score, num) { }

        ~AIPlayer() { }

        protected override void OnNewGame(int rows, int columns, int numberOfPlayers)
        {
            // The AI must somehow remeber what tile has been opened or not..
            OpenedMinefield = Enumerable.Repeat(false, rows * columns).ToArray();
        }

        protected override void OnMineOpened(PlayerNum playerNumber, Mine m, bool success) {
            // IFF the AI tried to open a mine that was already taken, success == false
            // In such a case, the AI must try again!
            if (success)
            {
                OpenedMinefield[m.index] = true;
            }
            else if (playerNumber == PlayerNum)
            {
                // Try again!
                HandleTurn(PlayerNum);
            }
        }

        protected override void HandleTurn(PlayerNum playerNumber)
        {
            // If it is not the AI:s turn, just return...
            if (playerNumber != PlayerNum)
                return;

            // This AI is really dumb and just randomly selects a mine and hopes for the best
            Random r = new Random();
            for (;;)
            {
                int index = r.Next(0, OpenedMinefield.Length - 1);

                // We can't open that one, try again
                if (OpenedMinefield[index])
                    continue;

                // Signal the controller to open the mine
                BaseController.OnOpenMine(index, PlayerNum, false);

                break;
            }
        }

        public override string GetPlayerType()
        {
            return "ai";
        }
    }
}
